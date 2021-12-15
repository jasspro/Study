public void BindGridLogger(int pagesize, string searchName)
    {
        try
        {
            gridExceptionLogger.Visible = true;
            ucGridPagerLogger.Visible = true;
            Int64 recourtCount = 0;

            DataTable dt = new DataTable();
            dt.Columns.Add("Index", typeof(int));
            dt.Columns.Add("errorLog", typeof(string));

            string DirectoryPath = string.Empty;
            if (ddlSourceType.SelectedValue == ((int)ErrorType.SMPPServer).ToString())
                DirectoryPath = Path.Combine(AppConfiguration.SMPPWebServerLogPath, ddllsttxt.SelectedItem.Text);
            else
                DirectoryPath = Path.Combine(AppConfiguration.SMPPWebClientLogPath, ddllsttxt.SelectedItem.Text);

            if (!string.IsNullOrEmpty(DirectoryPath))
            {
          
                List<List<string>> listOfList = new List<List<string>>();
                var list = new List<string>();

                using (var file = File.OpenRead(DirectoryPath))
                using (var reader = new StreamReader(file))
                {
                    long lineNumber = 0;

                    while (!reader.EndOfStream)
                    {
                        lineNumber = lineNumber + 1;
                        list.Add(reader.ReadLine());
                        if (list.Count >= pagesize)
                        {
                            listOfList.Add(list);
                            list = new List<string>();
                        }
                    }
                    listOfList.Add(list);
                    recourtCount = lineNumber;
                }
                int i = 0;
                long skipTill = (pagesize * pagenumberServerLog) - pagesize;
                long breakFrom = pagesize * pagenumberServerLog;

                foreach (List<string> oneList in listOfList.AsEnumerable().Reverse())
                {
                    foreach (string line in oneList.AsEnumerable().Reverse())
                    {
                        if (!string.IsNullOrEmpty(searchName))
                        {
                            if (line.Contains(searchName))
                            {
                                i++;
                                if (skipTill < i && i <= breakFrom)
                                    dt.Rows.Add(i, line);
                            }
                        }
                        else
                        {
                            i++;
                            if (skipTill < i && i <= breakFrom)
                                dt.Rows.Add(i, line);

                        }

                        if (i >= breakFrom)
                            break;
                    }
                }
            }

            gridExceptionLogger.DataSource = dt;
            gridExceptionLogger.DataBind();
            ucGridPagerLogger.GeneratePager(gridExceptionLogger.PageSize, Convert.ToInt16(pagenumberServerLog), recourtCount);
            lblRecordCount.Text = recourtCount.ToString();

        }
        catch (Exception ex)
        {
            ExceptionHandling.InsertExceptionMessage("Web", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, 1);
        }

    }
