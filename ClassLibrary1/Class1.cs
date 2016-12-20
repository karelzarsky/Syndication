using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        private void button1_Click(object sender, EventArgs e)
        {
            string connetionString = null;
            SqlConnection cnn;
            connetionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                cnn.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
