using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Cookie = System.Net.Cookie;

namespace 잔여백신_매크로
{
    public partial class Form1 : Form
    {
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        private ChromeDriver _driver = null;

        string Placeholder1 = "카카오메일 아이디,이메일,전화번호";
        string Placeholder2 = "비밀번호";
        string Placeholder3 = "도로명, 건물명, 지번검색";

        IReadOnlyCollection<OpenQA.Selenium.Cookie> cookies;
        string vaccine_type= "ANY";
        string map_data;
        string orgCode;

        bool success = false;

        JObject save_data=null;

        public Form1()
        {
            InitializeComponent();

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            /*
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            proc.StartInfo.Arguments = "https://www.kakao.com/main --new-window --remote-debugging-port=9222 --user-data-dir=C:\\Temp";
            proc.Start();
            */

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://vaccine-map.kakao.com/api/v3/vaccine/left_count_by_coords";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            
            request.Method = "POST";

            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add("Origin", "https://vaccine.kakao.com");
            request.Headers.Add("Accept-Language", "en-us");
            request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2";
            request.Referer = "https://vaccine.kakao.com/";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
  
            request.CookieContainer.Add(new Cookie("_T_ANO", "jSFW9xPlQnAOMtl7z46/8bjpDAyvvo9TT7/xiEyf3X8fQSlcJ93yJJgYtrXs18oEwPz7VuBh/m8/TqYKqhfcnAgc77bIxHGv8n+HObBQa5PfER2gsmvBwY+miLjzYJlC5/xcoglJ31FYXx/RstZ/mVmPquuyehoQTLekUrz5H49UuGyveAwPtaCVUD08IyJsevBs7V/1llASVa8Hjpv7/RSPq18X2RfjvaipA7ac/yIDZwtqsf/oqqubXPD2Nlsk1UORXTW1DXyw0x4yq3cOdVvWIg/y2USQltBc5EfLflmb9NjeIDWxMyxQbdroQ7DHSJjggN8N0Mf/nLrkRGRtGg==", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kawltea", "1628425494", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_karmtea", "1630952694", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_karmt", "kJJj-NXjWvDfU-NrJmO_PHZn_Sb0ao6bRJTOA3hsdWIYGjn6fDHdgoE1hezhUCWw", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kawlt", "Dy_5cEwkXVcc4PYERFKhYnfbHt5TIIjdgoebU70Umz2evgPQ3ccnclXoCkKikfbio-ubGJxMIybzHfJzCBk5tZaEgtHhWgS4VSairUre8Qt5yvZ6k7qN8HdUGfaxec2k", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("TIARA", "Aub6l2gVyM-fcutLy1PU78tIdnVilJV_KIA4eNmKI8Jwe.6V2Rc1aZ.Sy3R4x.1xov92i.nok9dEEriuYhXwMiHsk8kvcBxiTdoobmcunJo0", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kadu", "l3YSvaLFzjhHI7rT_1628349880346", "/", ".kakao.com"));
            

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"bottomRight\": {\"x\": 128.5422459701073, \"y\": 35.81989192989271}, \"onlyLeft\": false, \"order\": \"latitude\",\"topLeft\": {\"x\": 128.5249456298927, \"y\": 35.79719227010727}}";
                streamWriter.Write(json);
            }
            string vaccine_list;
            try
            {
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    vaccine_list = result;
                }
            }
            catch (WebException ex)
            {
                string pageContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                vaccine_list = pageContent;
            }

            var data = JObject.Parse(vaccine_list);

            //textBox2.Text = data["organizations"][3]["leftCounts"].ToString();

        }

        private void bunifuMaterialTextbox1_Enter(object sender, EventArgs e)
        {
            bunifuMaterialTextbox1.Refresh();
            bunifuMaterialTextbox2.Refresh();
            if (bunifuMaterialTextbox1.Text == "")
            { //텍스트박스 내용이 사용자가 입력한 값이 아닌 Placeholder일 경우에만, 커서 포커스일때 빈칸으로 만들기
                if (!bunifuMaterialTextbox1.ContainsFocus)
                {
                    bunifuMaterialTextbox1.ForeColor = Color.DarkGray;
                    bunifuMaterialTextbox1.Text = Placeholder1;
                }
            }
            if (bunifuMaterialTextbox2.Text == "")
            { //텍스트박스 내용이 사용자가 입력한 값이 아닌 Placeholder일 경우에만, 커서 포커스일때 빈칸으로 만들기
                if (!bunifuMaterialTextbox2.ContainsFocus)
                {
                    bunifuMaterialTextbox2.ForeColor = Color.DarkGray;
                    bunifuMaterialTextbox2.isPassword = false;
                    bunifuMaterialTextbox2.Text = Placeholder2;
                }
            }
            if (bunifuMaterialTextbox1.Text == Placeholder1)
            {
                bunifuMaterialTextbox1.ForeColor = Color.Black;
                bunifuMaterialTextbox1.Text = "";
            }
        }

        private void bunifuMaterialTextbox2_Enter(object sender, EventArgs e)
        {
            bunifuMaterialTextbox1.Refresh();
            bunifuMaterialTextbox2.Refresh();
            if (bunifuMaterialTextbox1.Text == "")
            { //텍스트박스 내용이 사용자가 입력한 값이 아닌 Placeholder일 경우에만, 커서 포커스일때 빈칸으로 만들기
                if (!bunifuMaterialTextbox1.ContainsFocus)
                {
                    bunifuMaterialTextbox1.ForeColor = Color.DarkGray;
                    bunifuMaterialTextbox1.Text = Placeholder1;
                }
            }
            if (bunifuMaterialTextbox2.Text == "")
            { //텍스트박스 내용이 사용자가 입력한 값이 아닌 Placeholder일 경우에만, 커서 포커스일때 빈칸으로 만들기
                if (!bunifuMaterialTextbox2.ContainsFocus)
                {
                    bunifuMaterialTextbox2.ForeColor = Color.DarkGray;
                    bunifuMaterialTextbox2.isPassword = false;
                    bunifuMaterialTextbox2.Text = Placeholder2;
                }
            }
            if (bunifuMaterialTextbox2.Text == Placeholder2)
            {
                bunifuMaterialTextbox2.ForeColor = Color.Black;
                bunifuMaterialTextbox2.Text = "";
            }
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            _options = new ChromeOptions();
            //_options.DebuggerAddress = "127.0.0.1:9222";
            _options.AddArgument("--headless");
            _options.AddArgument("--mute-audio");
            _options.AddArgument("disable-gpu");
            _options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2");

            _driver = new ChromeDriver(_driverService, _options);

            _driver.Navigate().GoToUrl("https://accounts.kakao.com/login?continue=https%3A%2F%2Fvaccine-map.kakao.com%2Fmap2%3Fv%3D1");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            //Thread.Sleep(3000);
            var element = _driver.FindElement(By.XPath("//*[@id='id_email_2']"));
            element.SendKeys(bunifuMaterialTextbox1.Text);

            element = _driver.FindElement(By.XPath("//*[@id='id_password_3']"));
            element.SendKeys(bunifuMaterialTextbox2.Text);
            element.SendKeys("\n");

            element = _driver.FindElement(By.XPath("//*[@id='errorAlert']/p"));
            textBox1.Text = element.Text;
            Thread.Sleep(1000);
            
            try
            {
                if (element.Text == "")
                {
                    panel2.Visible = true;
                    panel1.Visible = false;
                    Thread acceptThread = new Thread(() => certification());
                    acceptThread.IsBackground = true;   // 부모 종료시 스레드 종료
                    acceptThread.Start();
                }
                else
                {
                    label5.Visible = true;
                    bunifuThinButton21.Location = new Point(53, 230);
                    _driver.Quit();
                }
            }catch(Exception ex)
            {
                panel2.Visible = true;
                panel1.Visible = false;
                Thread acceptThread = new Thread(() => certification());
                acceptThread.IsBackground = true;   // 부모 종료시 스레드 종료
                acceptThread.Start();
            }


        }

        public void certification()
        {
            IWebElement element ;
            bool check_e=true;
                do
                {
                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            try
                            {
                                element = _driver.FindElement(By.XPath("//*[@id='pageLoginPoll']/div/strong"));
                                label1.Text = element.Text;
                                //element = _driver.FindElement(By.XPath("//*[@id='pageLoginPoll']/div/p"));
                                //label2.Text = element.Text;
                            }
                            catch (Exception ex)
                            {
                                check_e = false;
                            }

                        }));
                    }
                    else
                    {
                        try
                        {
                            element = _driver.FindElement(By.XPath("//*[@id='pageLoginPoll']/div/strong"));
                            label1.Text = element.Text;
                            element = _driver.FindElement(By.XPath("//*[@id='pageLoginPoll']/div/p"));
                            label2.Text = element.Text;
                        }
                        catch (Exception ex)
                        {
                        check_e = false;
                        }
                    }
                } while (check_e);
            

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    panel2.Visible = false;
                    panel3.Visible = true;
                    user_info_loaded();
                }));
            }
            else
            {
                panel2.Visible = false;
                panel3.Visible = true;
                user_info_loaded();
            }
        }
        
        public void user_info_loaded()
        {
            _driver.Navigate().GoToUrl("https://vaccine.kakao.com/api/v1/user");
            var element = _driver.FindElement(By.XPath("/html/body/pre"));
            //string Data = _driver.PageSource;
            var json = JObject.Parse(element.Text);
            if (json["user"]["status"].ToString() == "NORMAL")
                label3.Text = json["user"]["name"].ToString()+ "님 안녕하세요.";
            else if(json["user"]["status"].ToString() == "UNKNOWN")
                label3.Text = "상태를 알 수 없는 사용자입니다. 1339 또는 보건소에 문의해주세요.";
            else if (json["user"]["status"].ToString() == "REFUSED")
                label3.Text = json["user"]["name"].ToString() + "님은 백신을 예약하고 방문하지 않은 사용자로 파악됩니다. 잔여백신 예약이 불가합니다.";
            else if (json["user"]["status"].ToString() == "ALREADY_RESERVED"|| json["user"][0]["status"].ToString() == "ALREADY_VACCINATED")
                label3.Text = json["user"]["name"].ToString() + "님은 이미 예약 또는 접종이 완료된 사용자입니다.";

            cookies = _driver.Manage().Cookies.AllCookies;
            
            /*
            foreach (var cookie in cookies)
            {
                request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
            */
            
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            if (vaccine_type != "VEN00013")
            {
                vaccine_type = "VEN00013";
                bunifuImageButton1.Image = Properties.Resources.Pfizer_s;
                bunifuImageButton2.Image = Properties.Resources.moderna;
                bunifuImageButton3.Image = Properties.Resources.AstraZeneca;
                bunifuImageButton4.Image = Properties.Resources.Janssen;
            }
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if (vaccine_type != "VEN00014")
            {
                vaccine_type = "VEN00014";
                bunifuImageButton1.Image = Properties.Resources.Pfizer;
                bunifuImageButton2.Image = Properties.Resources.moderna_s;
                bunifuImageButton3.Image = Properties.Resources.AstraZeneca;
                bunifuImageButton4.Image = Properties.Resources.Janssen;
            }
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            if (vaccine_type != "VEN00015")
            {
                vaccine_type = "VEN00015";
                bunifuImageButton1.Image = Properties.Resources.Pfizer;
                bunifuImageButton2.Image = Properties.Resources.moderna;
                bunifuImageButton3.Image = Properties.Resources.AstraZeneca_s;
                bunifuImageButton4.Image = Properties.Resources.Janssen;
            }
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            if (vaccine_type != "VEN00016")
            {
                vaccine_type = "VEN00016";
                bunifuImageButton1.Image = Properties.Resources.Pfizer;
                bunifuImageButton2.Image = Properties.Resources.moderna;
                bunifuImageButton3.Image = Properties.Resources.AstraZeneca;
                bunifuImageButton4.Image = Properties.Resources.Janssen_s;
            }
        }

        private void bunifuMetroTextbox1_Enter(object sender, EventArgs e)
        {
            bunifuMetroTextbox1.Refresh();
            
            if (bunifuMetroTextbox1.Text == "")
            { //텍스트박스 내용이 사용자가 입력한 값이 아닌 Placeholder일 경우에만, 커서 포커스일때 빈칸으로 만들기
                if (!bunifuMetroTextbox1.ContainsFocus)
                {
                    bunifuMetroTextbox1.ForeColor = Color.DarkGray;
                    bunifuMetroTextbox1.Text = Placeholder3;
                }
            }

            if (bunifuMetroTextbox1.Text == Placeholder3)
            {
                bunifuMetroTextbox1.ForeColor = Color.Black;
                bunifuMetroTextbox1.Text = "";
            }
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query="+ bunifuMetroTextbox1.Text+ "&key=AIzaSyAcI6PON-LL3gpo1sl5eAcfq9KeaIDYiVU";
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string Data = wc.DownloadString(url);

            var json = JObject.Parse(Data);
            string southeast_lng = (double.Parse(json["results"][0]["geometry"]["viewport"]["southwest"]["lng"].ToString())+0.001*bunifuSlider1.Value).ToString();
            string southeast_lat = (double.Parse(json["results"][0]["geometry"]["viewport"]["northeast"]["lat"].ToString())+0.001*bunifuSlider1.Value).ToString();
            string northwest_lng = (double.Parse(json["results"][0]["geometry"]["viewport"]["northeast"]["lng"].ToString())-0.001*bunifuSlider1.Value).ToString();
            string northwest_lat = (double.Parse(json["results"][0]["geometry"]["viewport"]["southwest"]["lat"].ToString())-0.001*bunifuSlider1.Value).ToString();

            map_data ="{ \"bottomRight\": { \"x\":"+southeast_lng +", \"y\":"+ southeast_lat+"}, \"onlyLeft\": false, \"order\": \"latitude\",\"topLeft\": { \"x\":"+northwest_lng+", \"y\":"+northwest_lat+ "} }";

            //textBox1.Text= json["results"][0]["geometry"]["viewport"]["northeast"]["lat"].ToString();

            //find_vaccine();

            panel3.Visible = false;
            panel4.Visible = true;

            Thread acceptThread = new Thread(() => find_vaccine());
            acceptThread.IsBackground = true;   // 부모 종료시 스레드 종료
            acceptThread.Start();
        }

        public void retry_reservation()
        {
            string vaccine_list;

            string url = "https://vaccine.kakao.com/api/v2/reservation/retry";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();

            request.Method = "POST";

            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add("Origin", "https://vaccine.kakao.com");
            request.Headers.Add("Accept-Language", "en-us");
            request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2";
            request.Referer = "https://vaccine.kakao.com/";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");


            foreach (var cookie in cookies)
            {
                request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"from\":\"List\",\"vaccineCode\":\"" + vaccine_type + "\",\"orgCode\":" + orgCode + "}";
                streamWriter.Write(json);
            }

            try
            {
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    vaccine_list = result;
                }
            }
            catch (WebException ex)
            {
                string pageContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                vaccine_list = pageContent;
            }

            var data = JObject.Parse(vaccine_list);

            if (data["code"].ToString() == "NO_VACANCY")
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n";
                        save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n");
                    }));
                }
                else
                {
                    textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n";
                    save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n");
                }
            }
            if(data["code"].ToString() == "SUCCESS")
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n";
                        save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n");
                    }));
                }
                else
                {
                    textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n";
                    save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n");
                }
                success = true;
            }

        }

        public void try_reservation()
        {
            string vaccine_list;

            string url = "https://vaccine.kakao.com/api/v2/reservation";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();

            request.Method = "POST";

            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add("Origin", "https://vaccine.kakao.com");
            request.Headers.Add("Accept-Language", "en-us");
            request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2";
            request.Referer = "https://vaccine.kakao.com/";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");


            foreach (var cookie in cookies)
            {
                request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"from\":\"List\",\"vaccineCode\":\"" + vaccine_type + "\",\"orgCode\":" + orgCode + "}";
                streamWriter.Write(json);
            }

            try
            {
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    vaccine_list = result;
                }
            }
            catch (WebException ex)
            {
                string pageContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                vaccine_list = pageContent;
            }

            var data = JObject.Parse(vaccine_list);

            if (data["code"].ToString() == "NO_VACANCY")
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n";
                        save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n");
                    }));
                }
                else
                {
                    textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n";
                    save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("HH-mm-ss") + $" {orgCode} {vaccine_type} 예약 실패\r\n");
                }
                
            }
            else if(data["code"].ToString() == "SUCCESS")
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n";
                        save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n");
                    }));
                }
                else
                {
                    textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n";
                    save_log(@"c:\temp\vaccine_log.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + $"예약 성공\r\n");
                }
                success = true;
            }
            else if (data["code"].ToString() == "TIMEOUT")
                retry_reservation();
        }

        public void find_vaccine_type()
        {
            if (vaccine_type == "ANY")
            {
                try_reservation();
                return;
            }

            string vaccine_list;

            string url = "https://vaccine.kakao.com/api/v3/org/org_code/"+orgCode;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();

            request.Method = "GET";

            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add("Origin", "https://vaccine.kakao.com");
            request.Headers.Add("Accept-Language", "en-us");
            request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2";
            request.Referer = "https://vaccine.kakao.com";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");


            foreach (var cookie in cookies)
            {
                request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            /*
            request.CookieContainer.Add(new Cookie("_T_ANO", "jSFW9xPlQnAOMtl7z46/8bjpDAyvvo9TT7/xiEyf3X8fQSlcJ93yJJgYtrXs18oEwPz7VuBh/m8/TqYKqhfcnAgc77bIxHGv8n+HObBQa5PfER2gsmvBwY+miLjzYJlC5/xcoglJ31FYXx/RstZ/mVmPquuyehoQTLekUrz5H49UuGyveAwPtaCVUD08IyJsevBs7V/1llASVa8Hjpv7/RSPq18X2RfjvaipA7ac/yIDZwtqsf/oqqubXPD2Nlsk1UORXTW1DXyw0x4yq3cOdVvWIg/y2USQltBc5EfLflmb9NjeIDWxMyxQbdroQ7DHSJjggN8N0Mf/nLrkRGRtGg==", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kawltea", "1628425494", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_karmtea", "1630952694", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_karmt", "kJJj-NXjWvDfU-NrJmO_PHZn_Sb0ao6bRJTOA3hsdWIYGjn6fDHdgoE1hezhUCWw", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kawlt", "Dy_5cEwkXVcc4PYERFKhYnfbHt5TIIjdgoebU70Umz2evgPQ3ccnclXoCkKikfbio-ubGJxMIybzHfJzCBk5tZaEgtHhWgS4VSairUre8Qt5yvZ6k7qN8HdUGfaxec2k", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("TIARA", "Aub6l2gVyM-fcutLy1PU78tIdnVilJV_KIA4eNmKI8Jwe.6V2Rc1aZ.Sy3R4x.1xov92i.nok9dEEriuYhXwMiHsk8kvcBxiTdoobmcunJo0", "/", ".kakao.com"));
            request.CookieContainer.Add(new Cookie("_kadu", "l3YSvaLFzjhHI7rT_1628349880346", "/", ".kakao.com"));
            */

            try
            {
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    vaccine_list = result;
                }
            }
            catch (WebException ex)
            {
                string pageContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                vaccine_list = pageContent;
            }

            var data = JObject.Parse(vaccine_list);

            foreach (var vaccine_Count in data["lefts"])
            {
                //textBox1.Text += vaccine_Count["vaccineCode"].ToString()+" "+ vaccine_Count["leftCount"].ToString() + "\r\n";
                if(vaccine_Count["vaccineCode"].ToString()== vaccine_type&& vaccine_Count["leftCount"].ToString() != "0")
                {
                    try_reservation();
                    break;
                }
            }

        }


        public void find_vaccine()
        {
            string path = @"c:\temp\vaccine_log.txt";

            while (!success)
            {
                string url = "https://vaccine-map.kakao.com/api/v3/vaccine/left_count_by_coords";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = new CookieContainer();

                request.Method = "POST";

                request.Accept = "application/json, text/plain, */*";
                request.ContentType = "application/json;charset=utf-8";
                request.Headers.Add("Origin", "https://vaccine-map.kakao.com/");
                request.Headers.Add("Accept-Language", "en-us");
                request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 KAKAOTALK 9.4.2";
                request.Referer = "https://vaccine-map.kakao.com/";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");

                foreach (var cookie in cookies)
                {
                    request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                }

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = map_data;
                    streamWriter.Write(json);
                }

                string vaccine_list;

                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        vaccine_list = result;
                    }
                }
                catch (WebException ex)
                {
                    string pageContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                    vaccine_list = pageContent;
                }

                var data = JObject.Parse(vaccine_list);

                if (save_data == null)
                    save_data = data;
                else
                {
                    int n = 0;
                    foreach (var vaccine_Count in data["organizations"])
                    {
                        if (vaccine_Count["leftCounts"].ToString() == save_data["organizations"][n]["leftCounts"].ToString())
                        {
                            n++;
                            continue;
                        }
                        if (InvokeRequired)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                if (vaccine_Count["leftCounts"].ToString() != "0")
                                {
                                    orgCode = vaccine_Count["orgCode"].ToString();

                                textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + " 예약 가능한 기관을 발견하였습니다.\r\n";
                                save_log(path, DateTime.Now.ToString("HH-mm-ss") + " 예약 가능한 기관을 발견하였습니다.\r\n");
                                //find_vaccine_type();
                                try_reservation();
                                }
                            }));
                        }
                        else
                        {
                            if (vaccine_Count["leftCounts"].ToString() != "0")
                            {
                                orgCode = vaccine_Count["orgCode"].ToString();

                                textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + " 예약 가능한 기관을 발견하였습니다.\r\n";
                                save_log(path, DateTime.Now.ToString("HH-mm-ss") + " 예약 가능한 기관을 발견하였습니다.\r\n");
                                //find_vaccine_type();
                                try_reservation();
                            }
                        }
                        n++;
                        if (success)
                            break;
                    }

                    save_data = data;

                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            if (!success)
                            {
                                textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {data["organizations"].Count()}개의 접종기관중 예약 가능한 기관이 없습니다.\r\n";
                                save_log(path, DateTime.Now.ToString("HH-mm-ss") + $" {data["organizations"].Count()}개의 접종기관중 예약 가능한 기관이 없습니다.\r\n");
                            }
                        }));
                    }
                    else
                    {
                        if (!success)
                        {
                            textBox1.Text += DateTime.Now.ToString("HH-mm-ss") + $" {data["organizations"].Count()}개의 접종기관중 예약 가능한 기관이 없습니다.\r\n";
                            save_log(path, DateTime.Now.ToString("HH-mm-ss") + $" {data["organizations"].Count()}개의 접종기관중 예약 가능한 기관이 없습니다.\r\n");
                        }
                    }

                    if (textBox1.Text.Length > 10000)
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                textBox1.Text = "";
                            }));
                        }
                        else
                        {
                            textBox1.Text = "";
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        public void save_log(string p ,string s)
        {
            if (!File.Exists(p))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(p))
                {
                    sw.Write(s);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(p))
            {
                sw.Write(s);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try_reservation();
        }

        private void bunifuSlider1_ValueChanged(object sender, EventArgs e)
        {
            label10.Text = bunifuSlider1.Value.ToString();
        }
    }
}
