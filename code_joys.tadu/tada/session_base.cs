using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace tada {

public class session_base : IDisposable {
  i_connection_factory connection_factory;
  i_table_to_object_mapper mapper;
  IDbConnection connection;
  bool connection_is_shared = false;

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

  public List<t> all<t>(string sql) {
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

  public DataTable all(string sql) {
    IDataReader reader = null;
    var table = new DataTable();
    var connection = create_connection();
    try {
      if (connection.State != ConnectionState.Open)
        connection.Open();
      var command = connection.CreateCommand();
      command.CommandText = sql;
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

  IDbConnection create_connection() {
    return this.connection == null ? connection_factory.create() : this.connection;
  }


  public void Dispose() {
    Dispose(true);
  }

  protected virtual void Dispose(bool disposing) {
    if (!_disposed) {
      if (disposing) {
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