﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using code_joys;

namespace tada {

public class session_base : IDisposable {
  i_connection_factory connection_factory;
  i_table_to_object_mapper mapper;
  IDbConnection connection;
  IDbTransaction transaction;
  bool connection_is_shared = false;
  bool transaction_started = false;

  public session_base(i_connection_factory connection_factory, i_table_to_object_mapper mapper) { 
    this.connection_factory = connection_factory;
    this.mapper = mapper;
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
  }

  public void rollback() {
    transaction.Rollback();
    transaction_started = false;
  }

//  void test() {
//    var db = this;
//    //public session_base start_transaction() {}
//    //using (var db = new session().start_transaction()) {}
//    //db.sproc("get_user", params).all<user>();
//    //db.sproc("get_user").param("id", 1).all<user>();

//    int item_count, first_index, last_index;
//    var users = db
//      .cache("invalid users {0}-{1}".plug(first_index, last_index))
//      .page(first_index, last_index, out item_count)
//      .all<user>(@"where length(password) <= 6
//                   order by email");
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
    sql = process(typeof(t), sql);
    
    var table = all(sql);
    var list = mapper.map<t>(table);
    return list;
  }

  public t one<t>(string sql) {
    return all<t>(sql).First();
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
    // want to let user catch exception on their own; they may not want to rollback but rather handle exception
    //catch (Exception ex) {
    //  if (transaction_started && transaction != null)
    //    transaction.Rollback();
    //}
    finally {
      if (reader != null)
        reader.Close();
      if (!connection_is_shared && connection.State != ConnectionState.Closed)
        connection.Close();
    }
    return table;
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