using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;

namespace sam
{
    public partial class Form1 : Form
    {

        public TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string receive;
        public String text_to_send;
        public Array Con;
        public Array Key1;
        public Array Key2;
        public Int32 k1;
        public Int32  xor;
        public Int32 j;
        public Int64 k2;
        byte[] encrypted;
        public String msg;
        public String msge;
        public String encr;
        public String recencr;
        public String recdecr;
        public Int16 cnt;
        public Int32 Key3;
        public Int64 Key4;

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        UTF8Encoding utf8 = new UTF8Encoding();
        TripleDESCryptoServiceProvider tDES = new TripleDESCryptoServiceProvider();


        public Form1()
        {
            InitializeComponent();

            j = 7;
            textBox1.Text = "192.168.231.1";
            textBox2.Text = "192.168.231.1";
            String[] Con = { "JOHN", "KANE", "MIKE", "WILLSON", "PERRY", "KATE", "SMITH", "BUSH", "GEORGE" };
            textBox8.Text = Con[j];

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)             //server
        {

            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox3.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();                              //start receiving data in the bg
            backgroundWorker2.WorkerSupportsCancellation = true;             // ability to cancel this thread

        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)         //receive message
        {
            
            while (client.Connected)
            {

                try
                {
                  
                    cnt++;
                    encr = STR.ReadLine();
                    msge = STR.ReadLine();
                    text_to_send = STR.ReadLine();
                    Key4 = Convert.ToInt64(STR.ReadLine());

                   this.textBox5.Invoke(new MethodInvoker(delegate() { textBox12.AppendText(  encr+ "\n"); }));         //encr message
                    this.textBox10.Invoke(new MethodInvoker(delegate() { textBox11.AppendText( "Sam: " +text_to_send+  "\n"); }));  //chat box
                    this.textBox5.Invoke(new MethodInvoker(delegate() { textBox13.AppendText(msge + "\n"); }));             // decr message
                    this.textBox5.Invoke(new MethodInvoker(delegate() { textBox15.AppendText(Key4 + "\n"); }));                 //key 
           
                    
              
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());

                }


            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)   //send message
        {

            if (client.Connected)
            {
                cnt++;
                STW.WriteLine(encr);
                STW.WriteLine(text_to_send);
                STW.WriteLine(msge);
                STW.WriteLine(Key4);
                //STW.WriteLine(encrypted);
               this.textBox5.Invoke(new MethodInvoker(delegate() { textBox11.AppendText("Vam: " + msg + "\n"); }));
               textBox15.Text = "";

            }
            else
            {
                MessageBox.Show("Send Failed");
            }
            backgroundWorker2.CancelAsync();

        }

        private void button3_Click_1(object sender, EventArgs e)                  //connnect
        {
            client = new TcpClient();
            IPEndPoint IP_END = new IPEndPoint(IPAddress.Parse(textBox2.Text), int.Parse(textBox4.Text));

            try
            {

                client.Connect(IP_END);
                if (client.Connected)
                {
                    textBox7.Text = "";
                    textBox7.AppendText("Connected");
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync();                              //start receiving data in the bg
                    backgroundWorker2.WorkerSupportsCancellation = true;             // ability to cancel this thread


                }
            }
            catch (Exception x)
            {
                textBox7.Text = "";
                textBox7.AppendText("Not Connected");
                MessageBox.Show(x.Message.ToString());
            }

        }

        private void button1_Click(object sender, EventArgs e)             //send
        {
            textBox15.Text = "";
            if (textBox6.Text != "")
            {
                k1=k1+7;
                String[] Key1 = System.IO.File.ReadAllLines(@"F:\Volume(E)\592\Wordlist\file2.txt");
                textBox9.Text = Key1[k1];
 
                tDES.Key = md5.ComputeHash(utf8.GetBytes(textBox9.Text));
                tDES.Mode = CipherMode.ECB;
                tDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform trans = tDES.CreateEncryptor();
                encrypted = trans.TransformFinalBlock(utf8.GetBytes(textBox6.Text), 0, utf8.GetBytes(textBox6.Text).Length);
                textBox10.Text = "";
                textBox12.Text = "";
                textBox13.Text = "";
                textBox15.Text = "";
                textBox5.Text = "";
                textBox10.Text = BitConverter.ToString(encrypted);

                tDES.Key = md5.ComputeHash(utf8.GetBytes(textBox9.Text));
                tDES.Mode = CipherMode.ECB;
                tDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform tran = tDES.CreateDecryptor();
                msg = utf8.GetString(tran.TransformFinalBlock(encrypted, 0, encrypted.Length));
           
                this.textBox5.Invoke(new MethodInvoker(delegate() { textBox5.AppendText(msge + "\n"); }));
              
                     msge = msg;
                     textBox5.Text = msge;
                     text_to_send = textBox6.Text;
                     encr = textBox10.Text;
                     Key4 = Convert.ToUInt16(textBox9.Text);
                     switch (text_to_send)
                     {

                         case "hack":
                             msge = "#include<stdio.h>";
                             break;
                         case "clear":
                             msge = "Text Cleared";
                             textBox11.Text = "";
                             break;


                     }
                     backgroundWorker2.RunWorkerAsync();
                    
             }

            textBox6.Text = "";
            textBox13.Text = "";
            //textBox9.Text = "";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
