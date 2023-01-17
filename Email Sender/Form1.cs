using System.Net.Mail;
using System.Net;

namespace Email_Sender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            txtHTML.Text = fileContent;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    string[] fileContent = File.ReadAllLines(filePath);
                    listBox1.Items.AddRange(fileContent);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(string item in listBox1.Items)
            {               
                if(item.Contains("@") && item.Contains("."))
                {
                    try
                    {
                        richTextBox1.Text += "\r\n ----------------------------------- \r\n SENDING EMAIL TO " + item;
                        MailMessage message = new MailMessage();
                        SmtpClient smtp = new SmtpClient();
                        message.From = new MailAddress("noreply@coxino.ro");
                        message.To.Add(new MailAddress(item));
                        message.Subject = txtSubiect.Text;
                        message.IsBodyHtml = true; //to make message body as html  
                        message.Body = txtHTML.Text;
                        smtp.Port = 26;
                        smtp.Host = "mail.coxino.ro"; //for gmail host  
                        smtp.EnableSsl = false;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("noreply@coxino.ro", "1qazxsw23edc$RFV");
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);

                        richTextBox1.Text += "\r\n SENT EMAIL TO " + item;
                    }
                    catch (Exception e1)
                    {
                        richTextBox1.Text += "\r\n failed TO " + item;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    string[] fileContent = File.ReadAllLines(filePath);
                    parseEmails(fileContent);
                }
            }
        }


        int sentEmails = 0;
        int failEmails = 0;
        private void parseEmails(string[] fileContent)
        {
            foreach (string email in fileContent)
            {
                if (email.Contains("SENT EMAIL TO"))
                {
                    sentEmails++;
                }
                if(email.Contains("failed TO"))
                {
                    var emailA = email.Substring("failed TO ".Length);
                    listBox1.Items.Add(emailA);
                    failEmails++;
                }
            }

            MessageBox.Show($"Sent {sentEmails}, failed {failEmails}.");
        }
    }
}