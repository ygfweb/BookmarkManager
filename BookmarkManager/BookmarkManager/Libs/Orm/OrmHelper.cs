/******************
 * 约束：
 * 1. 实体类的主键名必须Id
 * 2. 实体类的属性名称必须与查询结果一一对应
 * 3. 批量操作必须显式开启事务
 * 4. 实例化该类会自动打开数据库连接
 * ****************/


using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Orm
{
    public class OrmHelper : IDisposable
    {
        protected SQLiteFactory ProviderFactory { get; set; }
        public SQLiteConnection Connection { get; private set; }
        public string Connectionstring { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        protected DbTransaction CurrentTransaction { get; set; }

        public OrmHelper(string connectionstring,string password = "")
        {
            ProviderFactory = SQLiteFactory.Instance;
            if (StringHelper.IsNullOrWhiteSpace(connectionstring))
            {
                throw new ArgumentNullException(nameof(connectionstring));
            }
            Connectionstring = connectionstring;
            this.Connection = this.ProviderFactory.CreateConnection() as SQLiteConnection;
            this.Connection.ConnectionString = connectionstring;
            this.Connection.SetPassword(password);
            this.Connection.Open();
        }

        /// <summary>
        /// 修改数据库密码
        /// </summary>
        public void ChangePassword(string newPassword)
        {
            if (StringHelper.IsNullOrWhiteSpace(newPassword))
            {
                this.Connection.SetPassword("");
            }
            else
            {
                this.Connection.ChangePassword(newPassword.Trim());
            }
        }


        /// <summary>
        /// 开启事务
        /// </summary>
        public Transaction BeginTransaction()
        {
            if (this.CurrentTransaction == null)
            {
                this.CurrentTransaction = this.Connection.BeginTransaction();
                return new Transaction(this);
            }
            return new Transaction(this);
        }

        /// <summary>
        /// 查询多个记录
        /// </summary>
        public async Task<List<T>> SelectMore<T>(string selectSql, object paramObject = null) where T : class, new()
        {
            DbCommand command = this.Connection.CreateCommand();
            command.CommandText = selectSql;
            ReflectionHelper.GenerateParameters(command, paramObject);
            DbDataReader reader = await command.ExecuteReaderAsync();
            List<T> list = ReflectionHelper.ConvertToList<T>(reader);
            return list;
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        public async Task<T> SelectOne<T>(string selectSql, object paramObject = null) where T : class, new()
        {
            DbCommand command = this.Connection.CreateCommand();
            command.CommandText = selectSql;
            ReflectionHelper.GenerateParameters(command, paramObject);
            DbDataReader reader = await command.ExecuteReaderAsync();
            List<T> list = ReflectionHelper.ConvertToList<T>(reader);
            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// 查询标量值（返回Int类型）
        /// </summary>
        public async Task<int> SelectIntScalar(string selectSql, object paramObject = null)
        {
            DbCommand command = this.Connection.CreateCommand();
            command.CommandText = selectSql;
            ReflectionHelper.GenerateParameters(command, paramObject);
            object result = await command.ExecuteScalarAsync();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        ///  查询标量值（返回Double类型）
        /// </summary>
        public async Task<double> SelectDoubleScalar(string selectSql, object paramObject = null)
        {
            DbCommand command = this.Connection.CreateCommand();
            command.CommandText = selectSql;
            ReflectionHelper.GenerateParameters(command, paramObject);
            object result = await command.ExecuteScalarAsync();
            return result == null ? 0 : Convert.ToDouble(result);
        }

        /// <summary>
        /// 执行自定义SQL语句（非查询）
        /// </summary>
        public async Task<int> ExecuteNonQuery(string sql, object paramObject = null)
        {
            DbCommand command = this.CreateExecCommand(sql);
            if (paramObject != null)
            {
                ReflectionHelper.GenerateParameters(command, paramObject);
            }
            return await command.ExecuteNonQueryAsync();
        }


        /// <summary>
        /// 创建可执行命令（增加、删除、修改，不能是查询）
        /// </summary>
        /// <param name="sql"></param>
        private DbCommand CreateExecCommand(string sql)
        {
            if (StringHelper.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            DbCommand command = this.Connection.CreateCommand();
            command.CommandText = sql;
            if (this.CurrentTransaction != null)
            {
                command.Transaction = this.CurrentTransaction;
            }
            return command;
        }

        private async Task<int> _batch<T>(List<T> objList, string sql) where T : class, new()
        {
            int result = 0;
            if (this.CurrentTransaction == null)
            {
                throw new InvalidOperationException("批量操作必须开启事务");
            }
            foreach (T obj in objList)
            {
                DbCommand command = this.CreateExecCommand(sql);
                ReflectionHelper.GenerateParameters<T>(command, obj);
                int count = await command.ExecuteNonQueryAsync();
                result = count + result;
            }
            return result;
        }

        public async Task<int> Insert<T>(T obj) where T : class, new()
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            DbCommand command = this.CreateExecCommand(ReflectionHelper.GenerateInsertql<T>());
            ReflectionHelper.GenerateParameters<T>(command, obj);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> Insert<T>(List<T> objList) where T : class, new()
        {
            if (objList == null || objList.Count == 0)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            string sql = ReflectionHelper.GenerateInsertql<T>();
            return await _batch<T>(objList, sql);
        }

        public async Task<int> Delete<T>(T obj) where T : class, new()
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            string sql = ReflectionHelper.GenerateDeleteSql<T>();
            DbCommand command = this.CreateExecCommand(sql);
            ReflectionHelper.GenerateParameters<T>(command, obj);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> Delete<T>(List<T> objList) where T : class, new()
        {
            if (objList == null || objList.Count == 0)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            string sql = ReflectionHelper.GenerateDeleteSql<T>();
            return await _batch<T>(objList, sql);
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        public async Task<int> Update<T>(T obj) where T : class, new()
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            string sql = ReflectionHelper.GenerateUpdateSql<T>();
            DbCommand command = this.CreateExecCommand(sql);
            ReflectionHelper.GenerateParameters<T>(command, obj);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> Update<T>(List<T> objList) where T : class, new()
        {
            if (objList == null || objList.Count == 0)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            string sql = ReflectionHelper.GenerateUpdateSql<T>();
            return await _batch<T>(objList, sql);
        }

        public void Dispose()
        {
            if (this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
            this.Connection.Dispose();
        }

        /// <summary>
        /// 事务
        /// </summary>
        public class Transaction : IDisposable
        {
            private OrmHelper OrmHelper { get; set; }

            public Transaction(OrmHelper ormHelper)
            {
                OrmHelper = ormHelper ?? throw new ArgumentNullException(nameof(ormHelper));
            }

            public void Commit()
            {
                this.OrmHelper.CurrentTransaction.Commit();
            }
            public void Rollback()
            {
                this.OrmHelper.CurrentTransaction.Rollback();
            }

            public void Dispose()
            {
                this.OrmHelper.CurrentTransaction.Dispose();
                this.OrmHelper.CurrentTransaction = null;
            }
        }
    }
}


