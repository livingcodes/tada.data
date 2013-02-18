using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using code_joys;

namespace tada {

public class session_base : IDisposable {
  i_connection_factory connection_factory;
  i_table_to_object_mapper mapper;
  i_cache _cache;
  IDbConnection connection;
  IDbTransaction transaction;
  bool connection_is_shared = false;
  bool transaction_started = false;

  public session_base(
    i_connection_factory connection_factory, 
    i_table_to_object_mapper mapper, 
    i_cache cache
  ) { 
    this.connection_factory = connection_factory;
    this.mapper = mapper;
    _cache = cache;
  }

  public session_base open_connection() {
    connection = connection_factory.create();
    connection.Open();
    connection_is_shared = true;
    return this;
  }

  public session_base start_transaction() {
    open_connection();
    transaction = connection.BeginTransaction();
    transaction_started = true;
    return this;
  }

  public void commit() {
    transaction.Commit();
    transaction_started = false;
    close_connection();
  }

  public void rollback() {
    transaction.Rollback();
    transaction_started = false;
    if (connection != null && connection.State != ConnectionState.Closed)
      connection.Close();
  }

//  void test() {
//    var user = db.sproc("get_user", id).one<user>();
//    var users = db.sproc("get_user").param("id", id).all<user>();
//    var result = db.sproc("login", email, password)
//    if (result.has_error)
//      display(result.error.message)
//      return;
//    else
//      user = result.user


//    var users = db
//      .page(first_index, last_index, out item_count)
//      .all<user>(@"where length(password) <= 6
//                   order by email");

//      .all<user>("where length(password) <= 6", 
//                 start_index, end_index, out count);

//      db.page_item_count
//  }

  public virtual string process(Type type, string sql) {
    if (!sql.ToLower().StartsWith("where "))
      return sql;

    var mapping = mapper.table_mappings.Where(m => m.type == type).First();
    var new_sql = "select * from {0} {1}"
      .plug(mapping.table, sql);
    return new_sql;
  }

  public List<t> all<t>(string sql) {
    if (cache_key != null && cache_result != null && !cached_one) {
      cache_key = null;
      return (List<t>)cache_result;
    }

    sql = process(typeof(t), sql);
    
    var table = all(sql);
    var list = mapper.map<t>(table);

    if (cache_key != null && !cached_one) {
      cache_key = null;
      _cache.set(cache_key, list);
    }
    return list;
  }

  public t one<t>(string sql) {
    if (cache_key != null) {
      if (cache_result != null) {
        cache_key = null;
        return (t)cache_result;
      }
      cached_one = true;
    }

    var result = all<t>(sql).First();
    
    if (cache_key != null) {
      _cache.set(cache_key, result);
      cache_key = null;
      cached_one = false;
    }

    return result;
  }

  public int execute(string sql) {
    // todo: parse parameters from sql
    int rows_affected;
    var connection = create_connection();
      
    try {
      if (connection.State != ConnectionState.Open)
        connection.Open();
      var command = connection.CreateCommand();
      if (transaction_started)
        command.Transaction = transaction;
      command.CommandText = sql;
      rows_affected = command.ExecuteNonQuery();
    }
    finally {
      if (!connection_is_shared && connection.State != ConnectionState.Closed)
        connection.Close();
    }
    return rows_affected;
  }

  public void close_connection() {
    connection.Close();
    connection = null;
    connection_is_shared = false;
  }

  IDictionary<string, string> param_info = new Dictionary<string, string>();

  public session_base param(string name, object value) {
    param_info.Add(name, value.ToString());
    return this;
  }

  public DataTable all(string sql) {   
    IDataReader reader = null;
    var table = new DataTable();
    var connection = create_connection();
    try {
      if (connection.State != ConnectionState.Open)
        connection.Open();
      var command = connection.CreateCommand();
      if (transaction_started)
        command.Transaction = transaction;
      command.CommandText = sql;

      foreach (var p in param_info) {
        var parameter = command.CreateParameter();
        parameter.ParameterName = p.Key;
        parameter.Value = p.Value;
        command.Parameters.Add(parameter);
      }

      reader = command.ExecuteReader();
      table.Load(reader);
    }
    finally {
      if (reader != null)
        reader.Close();
      if (!connection_is_shared && connection.State != ConnectionState.Closed)
        connection.Close();
    }
    return table;
  }

  object cache_result;
  string cache_key;
  bool cached_one = false;

  public session_base cache(string key) {
    cache_result = _cache.get(key);
    cache_key = key;
    return this;
  }

  IDbConnection create_connection() {
    return this.connection == null ? connection_factory.create() : this.connection;
  }


  public void Dispose() {
    Dispose(true);
  }

  protected virtual void Dispose(bool disposing) {
    if (!_disposed) {
      if (disposing) {
        if (transaction_started) {
          transaction.Rollback();
          transaction_started = false;
        }
        if (connection != null && connection.State != ConnectionState.Closed)
          connection.Close();
      }
      connection_is_shared = false;
      connection = null;
      _disposed = true;
    }
  }

  protected bool _disposed;
}
}