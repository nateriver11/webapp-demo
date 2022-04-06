using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ASMWebTest1Project
{
    public class DBLib
    {
        // khai baos biến thành viên 
        private SqlConnection cnn;
        private SqlCommand cmd;

        public DBLib()
        {
            // khai báo chuỗi kết nối 
            string strCnn = @"Server=.;Database=QUANLYKHACHSAN;Integrated security=true;";
            // khai báo đối tượng sql connection
            cnn = new SqlConnection(strCnn);
            cmd = new SqlCommand();
            cmd.Connection = cnn;

        }

        //phương thức mở kết nối 
        public void Open()
        {
            if (cnn.State != System.Data.ConnectionState.Open)
            {
                cnn.Open();
            }
        }

        //phương thức đòng kết nối 
        public void Close()
        {
            if (cnn.State != System.Data.ConnectionState.Closed)
            {
                cnn.Close();

            }
        }

        // phương thức thực thi lệnh sql , với tên và các kiểu lệnh
        public bool ExecuteNonQuery(string cmdText, CommandType cmdType)
        {
            int count = 0;
            try
            {
                Open();
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return count > 0;
        }

        // phương thức truyền tham số
        public void AddParameter(string name, object value)
        {
            SqlParameter para = new SqlParameter();
            para.ParameterName = name;
            para.Value = value;
            cmd.Parameters.Add(para);
        }
        public void AddParameters(string name, object value, ParameterDirection direction)
        {
            SqlParameter para = new SqlParameter();
            para.ParameterName = name;
            para.Value = value;
            para.Direction = direction;
            cmd.Parameters.Add(para);
        }

        // phuonwg thuc lay tham so dau ra
        public int GetParameter(string parameterName)
        {
            return (int)cmd.Parameters[parameterName].Value;

        }

        public DataTable FillDataTable(string cmdText, CommandType cmdType)
        {
            DataTable table = null;
            try
            {
                Open();
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                table = new DataTable();
                adapter.Fill(table);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return table;
        }

        public DataTable FillDataTable(string cmdText, CommandType cmdType,
            string[] arrPara, object[] arrValue, SqlDbType[] arrSqlDbType)
        {
            DataTable table = null;
            try
            {
                Open();
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                // truyen tham so
                SqlParameter para = null;
                for (int i = 0; i < arrPara.Length; i++)
                {
                    para = new SqlParameter();
                    para.ParameterName = arrPara[i];
                    para.Value = arrValue[i];
                    para.SqlDbType = arrSqlDbType[i];
                    cmd.Parameters.Add(para);

                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                table = new DataTable();
                adapter.Fill(table);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return table;
        }

    }
}