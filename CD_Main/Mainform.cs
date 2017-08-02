using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CD_Main
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable authorTable = Utilities.AuthorTable;
                cBoxAuthor.DataSource = authorTable;
                cBoxAuthor.DisplayMember = "nameAuthor";
                DataTable opusTable = Utilities.OpusTable;
                cBoxTitle.DataSource = opusTable;
                cBoxTitle.DisplayMember = "titleOpus";
                toolStripStatusLabel1.Text = "";
                cBoxAuthor.Enabled = true;
                cBoxTitle.Enabled = true;
                btnAuthor.Enabled = true;
                btnTitle.Enabled = true;
                dataGridView1.Enabled = true;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
            finally
            {
                // Chargement des settings
                if (Properties.Settings.Default.WindowMaximized)
                {
                    WindowState = FormWindowState.Maximized;
                    Location = Properties.Settings.Default.WindowLocation;
                    Size = Properties.Settings.Default.WindowSize;
                }
                else if (Properties.Settings.Default.WindowMinimized)
                {
                    WindowState = FormWindowState.Minimized;
                    Location = Properties.Settings.Default.WindowLocation;
                    Size = Properties.Settings.Default.WindowSize;
                }
                else
                {
                    Location = Properties.Settings.Default.WindowLocation;
                    Size = Properties.Settings.Default.WindowSize;
                }
            }
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    Properties.Settings.Default.WindowLocation = Location;
                    Properties.Settings.Default.WindowSize = Size;
                    Properties.Settings.Default.WindowMaximized = false;
                    Properties.Settings.Default.WindowMinimized = false;
                    break;
                case FormWindowState.Maximized:
                    Properties.Settings.Default.WindowLocation = RestoreBounds.Location;
                    Properties.Settings.Default.WindowSize = RestoreBounds.Size;
                    Properties.Settings.Default.WindowMaximized = true;
                    Properties.Settings.Default.WindowMinimized = false;
                    break;
                case FormWindowState.Minimized:
                    Properties.Settings.Default.WindowLocation = RestoreBounds.Location;
                    Properties.Settings.Default.WindowSize = RestoreBounds.Size;
                    Properties.Settings.Default.WindowMaximized = false;
                    Properties.Settings.Default.WindowMinimized = true;
                    break;
                default:
                    break;
            }
            Properties.Settings.Default.Save();
        }

        private void btnAuthor_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = Utilities.FindByAuthorName(cBoxAuthor.Text);
                dataGridView1.Columns["Perdu"].Visible = false;
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void btnTitle_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = Utilities.FindByOpusTitle(cBoxTitle.Text);
                dataGridView1.Columns["Perdu"].Visible = false;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }
    }
}
