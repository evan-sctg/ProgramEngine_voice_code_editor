using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramEngine
{
    public partial class WebView : Form
    {
        public WebView()
        {
            InitializeComponent();
        }
        
        

        private void button1_Click(object sender, EventArgs e)
        {
            NavigateWebView(textBox1.Text);
        }

        private void WebView_Load(object sender, EventArgs e)
        {

        }

        public void NavigateWebView(string TOURL)
        {
            if (TOURL.EndsWith(".com") || TOURL.EndsWith(".net") || TOURL.EndsWith(".org") || TOURL.EndsWith(".gov") || TOURL.EndsWith(".edu"))
            {
                try
                {

                    webControl1.Source = new Uri(TOURL);
                }
                catch (System.UriFormatException ex)
                {


                    webControl1.Source = new Uri("http://"+TOURL);
                }

            }else
            {
               webControl1.Source = new Uri("https://www.google.com/search?q=" + TOURL);
            }

            textBox1.Text = webControl1.Source.ToString();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(this, new EventArgs());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //refresh
            webRefresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //next
            if (webControl1.CanGoForward())
            {
                webControl1.GoForward();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //back
            if (webControl1.CanGoBack())
            {
                webControl1.GoBack();
            }
        }

        public void webRefresh(){
            webControl1.Source = new Uri(webControl1.Source.ToString());
            }
    }
}
