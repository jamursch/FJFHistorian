using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace AngularJSExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult loadTournaments()
        {                                   
           return Json(DisplayTournament(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadGolfers()
        {
           return Json(DisplayGolfers(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadActiveGolfers()
        {
            return Json(DisplayActiveGolfers(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadCourses()
        {
            return Json(DisplayCourses(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadRounds(string id)
        {
            return Json(DisplayLeaderboardRounds(id), JsonRequestBehavior.AllowGet);         
        }

        [HttpGet]
        public ActionResult loadFinalRounds(string id)
        {
            return Json(DisplayFinalTournamentRounds(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult displayGolferInfo(string id)
        {
            return Json(DisplayGolfer(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadGolferRounds(string id)
        {
            return Json(LoadGolferRounds(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult editTournament(string id)
        {
            return Json(DisplayTournament(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public String updateTournament(TournamentViewModel tournament)
        {
            return UpdateTournamentInDB(tournament);
        }

        [HttpPost]
        public String createBlankRound(GolferTournamentViewModel golferTournament)
        {
            return CreateBlankRoundInDB(golferTournament.Golfer, golferTournament.Tournament);
        }

        [HttpPost]
        public String addGolferTournamentRounds(GolferTournamentViewModel golferTournament)
        {
            return CreateGolferTournamentRoundsInDB(golferTournament.Rounds, golferTournament.Tournament);
        }

        [HttpPost]
        public String updateGolfer(GolferViewModel golfer)
        {
            return UpdateGolferInDB(golfer);
        }

        [HttpPost]
        public ActionResult calculateHandicap(GolferViewModel golfer)
        {
            return Json(CalculateNewHandicap(golfer), JsonRequestBehavior.AllowGet);
        }

        #region Tournament Signatures

        public string SaveTournamentInfo(TournamentViewModel tournament, string id)
        {
            string strSQL = "";

             if (string.IsNullOrWhiteSpace(id))
            {
                 // New Tournament
                strSQL = "INSERT INTO Tournaments (Id, Display_Name, FJF_Champion, Senior_Champion, Scramble_Champions, Tournament_Round_1_Date, Tournament_Round_2_Date, Tournament_Round_3_Date, Tournament_Round_4_Date, Tournament_Round_1_Course, Tournament_Round_2_Course, Tournament_Round_3_Course,Tournament_Round_4_Course, Status) VALUES (" + SQLValuePrep(id) + ", " + SQLValuePrep(tournament.Title) + ", " + SQLValuePrep(tournament.Champion) + ", " + SQLValuePrep(tournament.SeniorChampion) + ", " + SQLValuePrep(tournament.ScrambleChampions) + ", " + tournament.Round1_Date + ", " + tournament.Round2_Date + ", " + tournament.Round3_Date + ", " + tournament.Round4_Date + ", " + SQLValuePrep(tournament.Round1_Course) + ", " + SQLValuePrep(tournament.Round2_Course) + ", " + SQLValuePrep(tournament.Round3_Course) + ", " + SQLValuePrep(tournament.Round4_Course) + ", " + "1" + ")";
            }
            else
            {
                 // Existing Tournament
                strSQL = "UPDATE Tournaments SET Display_Name = " + tournament.Title + ", FJF_Champion = " + tournament.Champion + ", Senior_Champion = " + tournament.SeniorChampion + ", Scramble_Champions = " + tournament.ScrambleChampions + ", Tournament_Round_1_Date = " + tournament.Round1_Date + ", Tournament_Round_2_Date = " + tournament.Round2_Date + ", Tournament_Round_3_Date = " + tournament.Round3_Date + ", Tournament_Round_4_Date = " + tournament.Round4_Date + ", Tournament_Round_1_Course = " + SQLValuePrep(tournament.Round1_Course) + ", Tournament_Round_2_Course = " + SQLValuePrep(tournament.Round2_Course) + ", Tournament_Round_3_Course = " + SQLValuePrep(tournament.Round3_Course) + ", Tournament_Round_4_Course = " + SQLValuePrep(tournament.Round4_Course) + ", Status = " + "1" + " " + "WHERE Id=" + SQLValuePrep(id);
            }
                      
            if (DoQuickUpdateQuery(strSQL) == 1)
            {
                 return "Success!";
            }
            else
            {
                return "Fail!";
            }
          
        }

        public TournamentViewModel DisplayTournament(string id)
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                // EDIT OR CREATE
                if (string.IsNullOrWhiteSpace(id))
                {
                    var Tournament = new TournamentViewModel
                    {
                        Id = string.Empty,
                        Title = DateTime.Now.Year.ToString() + " FJF Memorial Golf Outing",
                        Champion = string.Empty,
                        SeniorChampion = string.Empty,
                        ScrambleChampions = string.Empty,
                        Round1_Date = DateTime.Now.ToShortDateString(),
                        Round2_Date = DateTime.Now.ToShortDateString(),
                        Round3_Date = DateTime.Now.ToShortDateString(),
                        Round4_Date = DateTime.Now.ToShortDateString(),
                        Round1_Course = string.Empty,
                        Round2_Course = string.Empty,
                        Round3_Course = string.Empty,
                        Round4_Course = string.Empty,
                        CourseStatus = 1
                    };
                    return Tournament;
                }
                else
                {
                    strSQL = "SELECT * FROM Tournaments WHERE ID = '" + id + "'";
                    objDS = DoQuickSelectQuery(strSQL);

                    //var result = new TournamentViewModel();

                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow objTournamentRow in objDS.Tables[0].Rows)
                        {
                            var Tournament = new TournamentViewModel
                            {
                                Id = objTournamentRow["ID"].ToString(),
                                Title = objTournamentRow["Display_Name"].ToString(),
                                Champion = objTournamentRow["FJF_Champion"].ToString(),
                                SeniorChampion = objTournamentRow["Senior_Champion"].ToString(),
                                ScrambleChampions = objTournamentRow["Scramble_Champions"].ToString(),
                                Round1_Date = GetDate(objTournamentRow, "Tournament_Round_1_Date"),
                                Round2_Date = GetDate(objTournamentRow, "Tournament_Round_2_Date"),
                                Round3_Date = GetDate(objTournamentRow, "Tournament_Round_3_Date"),
                                Round4_Date = GetDate(objTournamentRow, "Tournament_Round_4_Date"),
                                Round1_Course = GetDBString(objTournamentRow, "Tournament_Round_1_Course"),
                                Round2_Course = GetDBString(objTournamentRow, "Tournament_Round_2_Course"),
                                Round3_Course = GetDBString(objTournamentRow, "Tournament_Round_3_Course"),
                                Round4_Course = GetDBString(objTournamentRow, "Tournament_Round_4_Course"),
                                CourseStatus = Convert.ToInt32(GetDBString(objTournamentRow, "Status"))
                            };

                            return Tournament;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return null;
                }
                
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }
        }

        public IEnumerable<TournamentViewModel>DisplayTournament()
        {
             System.Data.DataSet objDS = null;
             string strSQL = "";

             try {
              // Generate SQL Statement
               strSQL = "SELECT * FROM Tournaments ORDER BY Display_Name DESC";

               objDS = DoQuickSelectQuery(strSQL);

                 var result = new List<TournamentViewModel>();
                
                foreach (DataRow objTournamentRow in objDS.Tables[0].Rows) 
                {
                        var Tournament = new TournamentViewModel
                      {
                           Id = objTournamentRow["Id"].ToString(),
                           Title = objTournamentRow["Display_Name"].ToString(),
                           Champion = objTournamentRow["FJF_Champion"].ToString(),
                           SeniorChampion = objTournamentRow["Senior_Champion"].ToString(),
                           ScrambleChampions = objTournamentRow["Scramble_Champions"].ToString()
                       };

                        result.Add(Tournament);   

               }
                return result;
            } 
            finally 
            {
                this.DestroyDataSet(objDS);
            }
            
        }

        public IEnumerable<LeaderboardViewModel> DisplayLeaderboardRounds(string tournamentId)
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";
                      
            try
            {
                // Check for Rounds already assigned
               strSQL = "Select G.Last_Name, G.First_Name, G.Id AS Golfer_ID, G.Adjusted_Handicap, T.Display_Name, R.Tournament_Name, R.ID,  ISNULL(R.Round_Handicap,0) AS Round_Handicap, ISNULL(R.Round_Senior_Handicap,0) AS Round_Senior_Handicap, ISNULL(R.Rd1_Score_Total_Net,0) AS Rd1_Score_Total_Net, ISNULL(R.Rd2_Score_Total_Net,0) AS Rd2_Score_Total_Net, ISNULL(R.Rd3_Score_Total_Net,0) AS Rd3_Score_Total_Net, ISNULL(R.Rd4_Score_Total_Net,0) AS Rd4_Score_Total_Net, ISNULL(R.Rd1_Score_Total_Gross,0) AS Rd1_Score_Total_Gross,  ISNULL(R.Rd2_Score_Total_Gross,0) AS Rd2_Score_Total_Gross,  ISNULL(R.Rd3_Score_Total_Gross,0) AS Rd3_Score_Total_Gross,  ISNULL(R.Rd4_Score_Total_Gross,0) AS Rd4_Score_Total_Gross, ISNULL(R.Rd1_HandicapIndex,0) AS Rd1_HandicapIndex, ISNULL(R.Rd2_HandicapIndex,0) AS Rd2_HandicapIndex, ISNULL(R.Rd3_HandicapIndex,0) AS Rd3_HandicapIndex, ISNULL(R.Rd4_HandicapIndex,0) AS Rd4_HandicapIndex, ISNULL(R.Rd1_Score_Total_Senior_Net,0) AS Rd1_Score_Total_Senior_Net, ISNULL(R.Rd2_Score_Total_Senior_Net,0) AS Rd2_Score_Total_Senior_Net, ISNULL(R.Rd3_Score_Total_Senior_Net,0) AS Rd3_Score_Total_Senior_Net, ISNULL(R.Rd4_Score_Total_Senior_Net,0) AS Rd4_Score_Total_Senior_Net, (ISNULL(R.Rd1_Score_Total_Net,0) + ISNULL(R.Rd2_Score_Total_Net,0) + ISNULL(R.Rd3_Score_Total_Net,0) + ISNULL(R.Rd4_Score_Total_Net,0)) as Total_Net From Tournaments T Left outer join Rounds R on R.Tournament_ID = T.ID Left outer join Golfers G on G.ID = R.Golfer_ID Where G.Qualified = 1 and Rd1_Score_Total_Gross <> 0 and Rd2_Score_Total_Gross <> 0 and Rd3_Score_Total_Gross <> 0 and Rd4_Score_Total_Gross <> 0 and T.ID = '" + tournamentId + "'Order by Total_Net";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<LeaderboardViewModel>();

                // If we have Rows, we have Rounds
                if (objDS.Tables[0].Rows.Count > 0)
                {
                    // Fill LeaderboardViewModel with our Rounds
                    foreach (DataRow objLeaderboardRow in objDS.Tables[0].Rows)
                    {
                        var Leaderboard = new LeaderboardViewModel
                        {
                            Id = objLeaderboardRow["ID"].ToString(),
                            FirstName = objLeaderboardRow["First_Name"].ToString(),
                            LastName = objLeaderboardRow["Last_Name"].ToString(),
                            TournamentName = objLeaderboardRow["Tournament_Name"].ToString(),
                            GolferId = objLeaderboardRow["Golfer_ID"].ToString(),
                            Handicap = Convert.ToInt32(objLeaderboardRow["Round_Handicap"]),
                            Adusted_Handicap = Convert.ToInt32(objLeaderboardRow["Adjusted_Handicap"]),
                            Sr_Handicap = Convert.ToInt32(objLeaderboardRow["Round_Senior_Handicap"]),

                            Round1_Gross = Convert.ToInt32(objLeaderboardRow["Rd1_Score_Total_Gross"]),
                            Round2_Gross = Convert.ToInt32(objLeaderboardRow["Rd2_Score_Total_Gross"]),
                            Round3_Gross = Convert.ToInt32(objLeaderboardRow["Rd3_Score_Total_Gross"]),
                            Round4_Gross = Convert.ToInt32(objLeaderboardRow["Rd4_Score_Total_Gross"]),

                            Round1_Net = Convert.ToInt32(objLeaderboardRow["Rd1_Score_Total_Net"]),
                            Round2_Net = Convert.ToInt32(objLeaderboardRow["Rd2_Score_Total_Net"]),                          
                            Round3_Net = Convert.ToInt32(objLeaderboardRow["Rd3_Score_Total_Net"]),                        
                            Round4_Net = Convert.ToInt32(objLeaderboardRow["Rd4_Score_Total_Net"]),

                            Round1_SrNet = Convert.ToInt32(objLeaderboardRow["Rd1_Score_Total_Senior_Net"]),
                            Round2_SrNet = Convert.ToInt32(objLeaderboardRow["Rd2_Score_Total_Senior_Net"]),
                            Round3_SrNet = Convert.ToInt32(objLeaderboardRow["Rd3_Score_Total_Senior_Net"]),
                            Round4_SrNet = Convert.ToInt32(objLeaderboardRow["Rd4_Score_Total_Senior_Net"]),

                            Round1_Hcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd1_HandicapIndex"]),
                            Round2_Hcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd2_HandicapIndex"]),
                            Round3_Hcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd3_HandicapIndex"]),
                            Round4_Hcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd4_HandicapIndex"]),

                            // Missing Columns in the DB
                            //Round1_SrHcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd1_Senior_HandicapIndex"]),
                            //Round2_SrHcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd2_Senior_HandicapIndex"]),
                            //Round3_SrHcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd3_Senior_HandicapIndex"]),
                            //Round4_SrHcp_Index = Convert.ToDecimal(objLeaderboardRow["Rd4_Senior_HandicapIndex"]),

                            Total_Net = Convert.ToInt32(objLeaderboardRow["Total_Net"])

                        };

                        result.Add(Leaderboard);
                    }
                }
                else
                {
                    // AutoFill LeaderboardViewModel with Active Golfers                  
                    // Generate SQL Statement
                    strSQL = "SELECT * FROM Golfers WHERE Active = '1' ORDER BY Last_Name ASC";
                    objDS = DoQuickSelectQuery(strSQL);

                    foreach (DataRow objGolferRow in objDS.Tables[0].Rows)
                    {
                        System.Guid strId = System.Guid.NewGuid();

                        var Leaderboard = new LeaderboardViewModel
                        {
                            Id = strId.ToString(),
                            FirstName = objGolferRow["First_Name"].ToString(),
                            LastName = objGolferRow["Last_Name"].ToString(),
                            GolferId = objGolferRow["ID"].ToString(),
                            Handicap = Convert.ToInt32(GetDBString(objGolferRow, "Current_Handicap")),
                            Sr_Handicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Current_Handicap")),
                            Round1_Gross = 0,
                            Round1_Net = 0,
                            Round2_Gross = 0,
                            Round2_Net = 0,
                            Round3_Gross = 0,
                            Round3_Net = 0,
                            Round4_Gross = 0,
                            Round4_Net = 0,
                            Total_Net = 0                      

                        };

                        result.Add(Leaderboard);

                    }

                }
                return result;
                                              
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }

        }

        public IEnumerable<LeaderboardViewModel> DisplayFinalTournamentRounds(string tournamentId)
        {
            System.Data.DataSet objDS = null;
            System.Data.DataSet objDS2 = null;
            string strSQL = "";
            string strSQL2 = "";
            string strTournamentName = "";
          
            try
            {

                strSQL2 = "SELECT * FROM Tournaments WHERE ID = '" + tournamentId + "'";
                objDS2 = DoQuickSelectQuery(strSQL2);

                if (objDS2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow objTournamentRow in objDS2.Tables[0].Rows)
                    {
                        strTournamentName = objTournamentRow["Display_Name"].ToString();
                        break;
                    }

                }

                // Generate SQL Statement
                strSQL = "Select G.Last_Name, G.First_Name, G.Adjusted_Handicap, R.Round_Handicap, R.Round_Senior_Handicap, ISNULL(R.Rd1_Score_Total_Net,0) AS Rd1_Score_Total_Net, ISNULL(R.Rd2_Score_Total_Net,0) AS Rd2_Score_Total_Net, ISNULL(R.Rd3_Score_Total_Net,0) AS Rd3_Score_Total_Net, ISNULL(R.Rd4_Score_Total_Net,0) AS Rd4_Score_Total_Net, ISNULL(R.Rd1_Score_Total_Gross,0) AS Rd1_Score_Total_Gross,  ISNULL(R.Rd2_Score_Total_Gross,0) AS Rd2_Score_Total_Gross,  ISNULL(R.Rd3_Score_Total_Gross,0) AS Rd3_Score_Total_Gross,  ISNULL(R.Rd4_Score_Total_Gross,0) AS Rd4_Score_Total_Gross,(ISNULL(R.Rd1_Score_Total_Net,0) + ISNULL(R.Rd2_Score_Total_Net,0) + ISNULL(R.Rd3_Score_Total_Net,0) + ISNULL(R.Rd4_Score_Total_Net,0)) as Total_Net From Tournaments T Left outer join Rounds R on R.Tournament_ID = T.ID Left outer join Golfers G on G.ID = R.Golfer_ID Where G.Qualified = 1 and Rd1_Score_Total_Gross <> 0 and Rd2_Score_Total_Gross <> 0 and Rd3_Score_Total_Gross <> 0 and Rd4_Score_Total_Gross <> 0 and T.ID = '" + tournamentId + "'Order by Total_Net";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<LeaderboardViewModel>();

                if (objDS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow objLeaderboardRow in objDS.Tables[0].Rows)
                    {
                        var Leaderboard = new LeaderboardViewModel
                        {
                            Id = objLeaderboardRow["Golfer_ID"].ToString(),
                            FirstName = objLeaderboardRow["First_Name"].ToString(),
                            LastName = objLeaderboardRow["Last_Name"].ToString(),
                            TournamentName = strTournamentName,
                            GolferId = objLeaderboardRow["Golfer_ID"].ToString(),
                            Handicap = Convert.ToInt32(objLeaderboardRow["Round_Handicap"]),
                            Adusted_Handicap = Convert.ToInt32(objLeaderboardRow["Adjusted_Handicap"]),
                            Round1_Gross = Convert.ToInt32(objLeaderboardRow["Rd1_Score_Total_Gross"]),
                            Round1_Net = Convert.ToInt32(objLeaderboardRow["Rd1_Score_Total_Net"]),
                            Round2_Gross = Convert.ToInt32(objLeaderboardRow["Rd2_Score_Total_Gross"]),
                            Round2_Net = Convert.ToInt32(objLeaderboardRow["Rd2_Score_Total_Net"]),
                            Round3_Gross = Convert.ToInt32(objLeaderboardRow["Rd3_Score_Total_Gross"]),
                            Round3_Net = Convert.ToInt32(objLeaderboardRow["Rd3_Score_Total_Net"]),
                            Round4_Gross = Convert.ToInt32(objLeaderboardRow["Rd4_Score_Total_Gross"]),
                            Round4_Net = Convert.ToInt32(objLeaderboardRow["Rd4_Score_Total_Net"]),
                            Total_Net = Convert.ToInt32(objLeaderboardRow["Total_Net"])

                        };

                        result.Add(Leaderboard);

                    }

                }
                else
                {
                    var Leaderboard = new LeaderboardViewModel
                    {
                        // Id = objLeaderboardRow["Id"].ToString(),
                        FirstName = "",
                        LastName = "No Rounds Available Prior to 2001 ",
                        Handicap = 0,
                        Adusted_Handicap = 0,
                        Round1_Gross = 0,
                        Round1_Net = 0,
                        Round2_Gross = 0,
                        Round2_Net = 0,
                        Round3_Gross = 0,
                        Round3_Net = 0,
                        Round4_Gross = 0,
                        Round4_Net = 0,
                        Total_Net = 0

                    };

                    result.Add(Leaderboard);
                }


                return result;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }

        }

        public string UpdateTournamentInDB(TournamentViewModel objTournament)
        {
            System.Data.DataSet objDS = null;
            string strSQL = string.Empty;
            System.Guid strId;

            if (objTournament != null)
            {

                try
                {
                    // EDIT vs. CREATE
                    if (string.IsNullOrWhiteSpace(objTournament.Id))
                    {
                        strId = System.Guid.NewGuid();

                        strSQL = "INSERT INTO Tournaments (Id, Display_Name, FJF_Champion, Senior_Champion, Scramble_Champions, Tournament_Round_1_Date, Tournament_Round_2_Date, Tournament_Round_3_Date, Tournament_Round_4_Date, Tournament_Round_1_Course, Tournament_Round_2_Course, Tournament_Round_3_Course,Tournament_Round_4_Course, Status) VALUES (" + SQLValuePrep(strId.ToString()) + ", " + SQLValuePrep(objTournament.Title) + ", " + SQLValuePrep(objTournament.Champion) + ", " + SQLValuePrep(objTournament.SeniorChampion) + ", " + SQLValuePrep(objTournament.ScrambleChampions) + ", " + SQLValuePrep(objTournament.Round1_Date) + ", " + SQLValuePrep(objTournament.Round2_Date) + ", " + SQLValuePrep(objTournament.Round3_Date) + ", " + SQLValuePrep(objTournament.Round4_Date) + ", " + SQLValuePrep(objTournament.Round1_Course) + ", " + SQLValuePrep(objTournament.Round2_Course) + ", " + SQLValuePrep(objTournament.Round3_Course) + ", " + SQLValuePrep(objTournament.Round4_Course) + ", " + SQLValuePrep("1") + ")";
                    }
                    else
                    {
                        strSQL = "UPDATE Tournaments SET Display_Name =" + SQLValuePrep(objTournament.Title) + ", FJF_Champion = " + SQLValuePrep(objTournament.Title) + ", Senior_Champion = " + SQLValuePrep(objTournament.SeniorChampion) + ", Scramble_Champions = " + SQLValuePrep(objTournament.ScrambleChampions) + ", Tournament_Round_1_Date = " + SQLValuePrep(objTournament.Round1_Date) + ", Tournament_Round_2_Date = " + SQLValuePrep(objTournament.Round2_Date) + ", Tournament_Round_3_Date = " + SQLValuePrep(objTournament.Round3_Date) + ", Tournament_Round_4_Date = " + SQLValuePrep(objTournament.Round4_Date) + ", Tournament_Round_1_Course = " + SQLValuePrep(objTournament.Round1_Course) + ", Tournament_Round_2_Course = " + SQLValuePrep(objTournament.Round2_Course) + ", Tournament_Round_3_Course = " + SQLValuePrep(objTournament.Round3_Course) + ", Tournament_Round_4_Course = " + SQLValuePrep(objTournament.Round4_Course) + ", Status = " + SQLValuePrep("1") + " WHERE Id=" + SQLValuePrep(objTournament.Id);
                    }

                    // Run the SQL and Return if it's successful or not
                    if (DoQuickUpdateQuery(strSQL) == 1)
                    {
                        return "Success!";
                    }
                    else
                    {
                        return "Error!";
                    }
                }
                finally
                {
                    this.DestroyDataSet(objDS);
                }                
            }
            else
            {
                return "Error. Missing Tournament";
            }
        }

        public string AddGolferToTournament(GolferViewModel objGolfer, TournamentViewModel objTournament)
        {
            System.Collections.Specialized.StringCollection colSelectedGolfers = new System.Collections.Specialized.StringCollection();

            if (objGolfer != null)
            {

                if (colSelectedGolfers.Contains(objGolfer.Id) == false)
                {
                    colSelectedGolfers.Add(objGolfer.Id);
                }

                return "Success! Golfer Round Created";
            }
            else
            {
                return "Error. Missing Tournament";
            }
        }

        public string CreateBlankRoundInDB(GolferViewModel objGolfer, TournamentViewModel objTournament)
        {
            System.Collections.Specialized.StringCollection colSelectedGolfers = new System.Collections.Specialized.StringCollection();

            if (objGolfer != null)
            {

                if (colSelectedGolfers.Contains(objGolfer.Id) == false)
                {
                    colSelectedGolfers.Add(objGolfer.Id);
                }

                var Players = new List<TournamentPlayerViewModel>();
               // System.Web.HttpContext.Current.Session["TournamentPlayers"] = colSelectedGolfers;
                //colSelectedGolfers = System.Web.HttpContext.Current.Session["TournamentPlayers"];

                              

                return "Success! Golfer Round Created";
            }
            else
            {
                return "Error. Missing Tournament";
            }
        }

        public string CreateGolferTournamentRoundsInDB(IEnumerable<LeaderboardViewModel> objRounds, TournamentViewModel objTournament)
        {


            if (objRounds != null)
            {
                //Loop through the objGolfers Collection and update their rounds in the DB
                //SQL Goes Here
                foreach (var round in objRounds)
                {
                    UpdateGolferRoundInDB(round, objTournament);
                }

                return "Success! Golfer Round Created";
            }
            else
            {
                return "Error. Missing Tournament";
            }
        }

        public LeaderboardViewModel CalculateRoundNetIndex(LeaderboardViewModel objRound, TournamentViewModel objTournament)
        {
           // int intHandicap = objRound.Handicap;
            //System.Data.DataSet objDS = null;
            //string strSQL = string.Empty;

            try
            {
                // Calculate Scores
                // ------------------------------
                // ----ROUND 1 ----
                if (objRound.Round1_Gross > 0 )
                {
                    objRound.Round1_Net = (objRound.Round1_Gross - objRound.Handicap);

                    if (objRound.Sr_Handicap > 0)
                    {
                        objRound.Round1_SrNet = (objRound.Round1_Gross - objRound.Sr_Handicap);
                    }

                    objRound.Round1_Hcp_Index = Decimal.Parse(CalculateHandicapIndex(objTournament.Round1_Course, objRound.Round1_Gross.ToString(), false));
                }
                // ----ROUND 2 ----
                if (objRound.Round2_Gross > 0)
                {
                    objRound.Round2_Net = (objRound.Round2_Gross - objRound.Handicap);
                    objRound.Round2_Hcp_Index = Decimal.Parse(CalculateHandicapIndex(objTournament.Round2_Course, objRound.Round2_Gross.ToString(), false));
                }
                // ----ROUND 3 ----
                if (objRound.Round3_Gross > 0)
                {
                    objRound.Round3_Net = (objRound.Round3_Gross - objRound.Handicap);
                    objRound.Round3_Hcp_Index = Decimal.Parse(CalculateHandicapIndex(objTournament.Round3_Course, objRound.Round3_Gross.ToString(), false));
                }
                // ----ROUND 4 ----
                if (objRound.Round4_Gross > 0)
                {
                    objRound.Round4_Net = (objRound.Round4_Gross - objRound.Handicap);
                    objRound.Round4_Hcp_Index = Decimal.Parse(CalculateHandicapIndex(objTournament.Round4_Course, objRound.Round4_Gross.ToString(), false));
                }

            }
            catch
            {

            }
            return objRound;
        }

        private string CalculateHandicapIndex(string vstrCourseId, string vstrScore, bool vblnUseSrTees)
        {
            string strHandicapIndex = "";
            decimal dblRating = 0;
            int intSlope = 0;
            decimal dblHandicapIndex = 0;
            System.Data.DataSet objDS = GetBaseDataSet();
            string strSQL = "";
            int intScore = 0;

            strSQL = "SELECT * FROM Courses";

            // Get the Golfer Information from the DB
            objDS = DoQuickSelectQuery(strSQL);

            // Loop through the DataSet
            foreach (System.Data.DataRow objRow in objDS.Tables[0].Rows)
            {
                // See if we have a match for the course Id

                if (vstrCourseId == this.GetDBString(objRow, "ID"))
                {
                    if (vblnUseSrTees == false)
                    {
                        dblRating = GetDBDecimal(objRow, "Course_Rating");
                        intSlope = GetDBInteger(objRow, "Course_Slope");
                    }
                    else
                    {
                        dblRating = GetDBDecimal(objRow, "Sr_Course_Rating");
                        intSlope = GetDBInteger(objRow, "Sr_Course_Slope");
                    }

                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            
            // Handicap Formula
            // ((Score - Course Rating) x 113 / Slope Rating) * 0.96
            intScore = int.Parse(vstrScore);
            dblHandicapIndex = Decimal.Subtract(intScore, dblRating);
            dblHandicapIndex = Decimal.Multiply(dblHandicapIndex, 113);
            dblHandicapIndex = Decimal.Divide(dblHandicapIndex,intSlope);
            dblHandicapIndex = Decimal.Multiply(dblHandicapIndex, Decimal.Parse("0.96"));
                      

            // Assign to string that's returned
            strHandicapIndex = Convert.ToString(Math.Round(dblHandicapIndex, 2));

            return strHandicapIndex;

        }

        public void UpdateGolferRoundInDB(LeaderboardViewModel objRound, TournamentViewModel objTournament)
        {
            System.Data.DataSet objDS = null;
            string strSQL = string.Empty;
            string strGETSQL = string.Empty;
            string strRoundId = string.Empty;
            string strResult = string.Empty;
           // System.Guid strNewRoundId;

            try
            {
                // Add code to do the calcualtions for the Indexes, etc. prior to doing the Insert or update
                // UpdatePlayerRoundValues(objRound)
                objRound = CalculateRoundNetIndex(objRound, objTournament);

                // Determine if we already have a round for this golfer 
                strGETSQL = "SELECT * FROM Rounds WHERE Golfer_ID = '" + objRound.GolferId + "' AND Tournament_ID = '" + objTournament.Id + "'";
                objDS = DoQuickSelectQuery(strGETSQL);

                if (objDS.Tables[0].Rows.Count > 0)
                {
                    strRoundId = GetDBString(objDS.Tables[0].Rows[0], "ID");

                    // Generate your Update SQL Statement
                    strSQL = "UPDATE Rounds SET Golfer_ID = " + SQLValuePrep(objRound.GolferId) + ", Tournament_ID = " + SQLValuePrep(objTournament.Id) + ", Tournament_Name = " + SQLValuePrep(objTournament.Title) + ", Round_Handicap = " + SQLValuePrep(objRound.Handicap.ToString()) + ", Round_Senior_Handicap = " + SQLValuePrep(objRound.Sr_Handicap.ToString()) + ", Rd1_Course_ID = " + SQLValuePrep(objTournament.Round1_Course) + ", Rd1_Score_Total_Gross = " + SQLValuePrep(objRound.Round1_Gross.ToString()) + ", Rd1_Score_Total_Net = " + SQLValuePrep(objRound.Round1_Net.ToString()) + ", Rd1_Score_Total_Senior_Net = " + SQLValuePrep(objRound.Round1_SrNet.ToString()) + ", Rd1_HandicapIndex = " + SQLValuePrep(objRound.Round1_Hcp_Index.ToString()) + ", Rd1_Date = " + SQLValuePrep(objTournament.Round1_Date.ToString()) + ", Rd2_Course_ID = " + SQLValuePrep(objTournament.Round2_Course) + ", Rd2_Score_Total_Gross = " + SQLValuePrep(objRound.Round2_Gross.ToString()) + ", Rd2_Score_Total_Net = " + SQLValuePrep(objRound.Round2_Net.ToString()) + ", Rd2_Score_Total_Senior_Net = " + SQLValuePrep(objRound.Round2_SrNet.ToString()) + ", Rd2_HandicapIndex = " + SQLValuePrep(objRound.Round2_Hcp_Index.ToString()) + ", Rd2_Date = " + SQLValuePrep(objTournament.Round2_Date.ToString()) + ", Rd3_Course_ID = " + SQLValuePrep(objTournament.Round3_Course) + ", Rd3_Score_Total_Gross = " + SQLValuePrep(objRound.Round3_Gross.ToString()) + ", Rd3_Score_Total_Net = " + SQLValuePrep(objRound.Round3_Net.ToString()) + ", Rd3_Score_Total_Senior_Net = " + SQLValuePrep(objRound.Round3_SrNet.ToString()) + ", Rd3_HandicapIndex = " + SQLValuePrep(objRound.Round3_Hcp_Index.ToString()) + ", Rd3_Date = " + SQLValuePrep(objTournament.Round3_Date.ToString()) + ", Rd4_Course_ID = " + SQLValuePrep(objTournament.Round1_Course) + ", Rd4_Score_Total_Gross = " + SQLValuePrep(objRound.Round4_Gross.ToString()) + ", Rd4_Score_Total_Net = " + SQLValuePrep(objRound.Round4_Net.ToString()) + ", Rd4_Score_Total_Senior_Net = " + SQLValuePrep(objRound.Round4_SrNet.ToString()) + ", Rd4_HandicapIndex = " + SQLValuePrep(objRound.Round4_Hcp_Index.ToString()) + ", Rd4_Date = " + SQLValuePrep(objTournament.Round4_Date.ToString()) + " " + "WHERE Id=" + SQLValuePrep(strRoundId);
                }
                else
                {
                    // Generate a unique GUID ID
                    //strNewRoundId = System.Guid.NewGuid();

                   
                    // Generate INSERT Statement
                    strSQL = "INSERT INTO Rounds (Id, Golfer_ID, Tournament_ID, Rd1_Course_ID, Rd1_Score_Total_Gross, Rd1_Score_Total_Net, Rd2_Course_ID, Rd2_Score_Total_Gross, Rd2_Score_Total_Net, Rd3_Course_ID, Rd3_Score_Total_Gross, Rd3_Score_Total_Net, Rd4_Course_ID, Rd4_Score_Total_Gross, Rd4_Score_Total_Net, Rd1_Score_Total_Senior_Net, Rd2_Score_Total_Senior_Net, Rd3_Score_Total_Senior_Net, Rd4_Score_Total_Senior_Net, Round_Handicap, Round_Senior_Handicap, Rd1_HandicapIndex, Rd2_HandicapIndex, Rd3_HandicapIndex, Rd4_HandicapIndex, Tournament_Name,Rd1_Date, Rd2_Date, Rd3_Date, Rd4_Date ) VALUES (" + SQLValuePrep(objRound.Id) + ", " + SQLValuePrep(objRound.GolferId) + ", " + SQLValuePrep(objTournament.Id) + ", " + SQLValuePrep(objTournament.Round1_Course) + ", " + SQLValuePrep(objRound.Round1_Gross.ToString()) + ", " + SQLValuePrep(objRound.Round1_Net.ToString()) + ", " + SQLValuePrep(objTournament.Round2_Course) + ", " + SQLValuePrep(objRound.Round2_Gross.ToString()) + ", " + SQLValuePrep(objRound.Round2_Net.ToString()) + ", " + SQLValuePrep(objTournament.Round3_Course) + ", " + SQLValuePrep(objRound.Round3_Gross.ToString()) + ", " + SQLValuePrep(objRound.Round3_Net.ToString()) + ", " + SQLValuePrep(objTournament.Round4_Course) + ", " + SQLValuePrep(objRound.Round4_Gross.ToString()) + ", " + SQLValuePrep(objRound.Round4_Net.ToString()) + ", " + SQLValuePrep(objRound.Round1_SrNet.ToString()) + ", " + SQLValuePrep(objRound.Round2_SrNet.ToString()) + ", " + SQLValuePrep(objRound.Round3_SrNet.ToString()) + ", " + SQLValuePrep(objRound.Round4_SrNet.ToString()) + ", " + SQLValuePrep(objRound.Handicap.ToString()) + ", " + SQLValuePrep(objRound.Sr_Handicap.ToString()) + ", " + SQLValuePrep(objRound.Round1_Hcp_Index.ToString()) + ", " + SQLValuePrep(objRound.Round2_Hcp_Index.ToString()) + ", " + SQLValuePrep(objRound.Round3_Hcp_Index.ToString()) + ", " + SQLValuePrep(objRound.Round4_Hcp_Index.ToString()) + ", " + SQLValuePrep(objTournament.Title) + ", " + SQLValuePrep(objTournament.Round1_Date.ToString()) + ", " + SQLValuePrep(objTournament.Round2_Date.ToString()) + ", " + SQLValuePrep(objTournament.Round3_Date.ToString()) + ", " + SQLValuePrep(objTournament.Round4_Date.ToString()) + ")";

                }

                // Run the SQL and Return if it's successful or not
                if (DoQuickUpdateQuery(strSQL) == 1)
                {
                    // return "Success!";
                    strResult = "Success";
                }
                else
                {
                   // return "Error!";
                    strResult = "Error";
                }


            }catch{
                // What happened?
            }
        }


        #endregion

        #region Golfer Signatures

        public IEnumerable<RoundViewModel> LoadGolferRounds(string id)
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                // Generate SQL Statement
                strSQL = "SELECT * FROM Rounds WHERE Golfer_ID = '" + id + "' ORDER BY Tournament_Name DESC";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<RoundViewModel>();

                foreach (DataRow objRoundRow in objDS.Tables[0].Rows)
                {
                    var Golfer = new RoundViewModel
                    {
                        Id = objRoundRow["Id"].ToString(),
                        GolferId = objRoundRow["Golfer_ID"].ToString(),
                        Tournament_Name = objRoundRow["Tournament_Name"].ToString(),
                        Rd_Hcp = Convert.ToInt32(GetDBString(objRoundRow, "Round_Handicap")),
                        Rd_SrHcp = Convert.ToInt32(GetDBString(objRoundRow, "Round_Senior_Handicap")),
                        Rd1_Score_Gross = Convert.ToInt32(GetDBString(objRoundRow, "Rd1_Score_Total_Gross")),
                        Rd2_Score_Gross = Convert.ToInt32(GetDBString(objRoundRow, "Rd2_Score_Total_Gross")),
                        Rd3_Score_Gross = Convert.ToInt32(GetDBString(objRoundRow, "Rd3_Score_Total_Gross")),
                        Rd4_Score_Gross = Convert.ToInt32(GetDBString(objRoundRow, "Rd4_Score_Total_Gross")),

                        Rd1_Score_Net = Convert.ToInt32(GetDBString(objRoundRow, "Rd1_Score_Total_Net")),
                        Rd2_Score_Net = Convert.ToInt32(GetDBString(objRoundRow, "Rd2_Score_Total_Net")),
                        Rd3_Score_Net = Convert.ToInt32(GetDBString(objRoundRow, "Rd3_Score_Total_Net")),
                        Rd4_Score_Net = Convert.ToInt32(GetDBString(objRoundRow, "Rd4_Score_Total_Net")),

                        Rd1_Hcp_Index = Convert.ToDecimal(GetDBString(objRoundRow, "Rd1_HandicapIndex")),
                        Rd2_Hcp_Index = Convert.ToDecimal(GetDBString(objRoundRow, "Rd2_HandicapIndex")),
                        Rd3_Hcp_Index = Convert.ToDecimal(GetDBString(objRoundRow, "Rd3_HandicapIndex")),
                        Rd4_Hcp_Index = Convert.ToDecimal(GetDBString(objRoundRow, "Rd4_HandicapIndex")),
                                               
                        Round1_Date = Convert.ToString(GetDate(objRoundRow, "Rd1_Date")),
                        Round2_Date = Convert.ToString(GetDate(objRoundRow, "Rd2_Date")),
                        Round3_Date = Convert.ToString(GetDate(objRoundRow, "Rd3_Date")),
                        Round4_Date = Convert.ToString(GetDate(objRoundRow, "Rd4_Date"))

                      //  Round3_Date = Convert.ToDateTime(GetDBString(objRoundRow, "Rd3_Date")),
                      //  Round4_Date = Convert.ToDateTime(GetDBString(objRoundRow, "Rd4_Date")),
                                               
                    };

                    result.Add(Golfer);

                }
                return result;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }
        }
        public GolferViewModel DisplayGolfer(string id)
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                 // EDIT OR CREATE
                if (string.IsNullOrWhiteSpace(id))
                {
                    var Golfer = new GolferViewModel
                    {
                        Id = string.Empty,
                        FirstName = "New",
                        LastName = "Golfer",
                        Email = string.Empty,
                        Handicap = 0,
                        AdjustedHandicap = 0,
                        Wins = 0,
                        SeniorHandicap = 0,
                        SeniorAdjustedHandicap = 0,
                        SeniorWins = 0,
                        Qualified = "0",
                        Active = "1",
                        SeniorQualified = "0",
                        LastWin = 0,
                        LastSeniorWin = 0
                    };
                    return Golfer;
                }
                else
                {
                    strSQL = "SELECT * FROM Golfers WHERE ID = '" + id + "'";
                    objDS = DoQuickSelectQuery(strSQL);

                    //var result = new LeaderboardViewModel();

                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow objGolferRow in objDS.Tables[0].Rows)
                        {
                            var Golfer = new GolferViewModel
                            {
                                Id = objGolferRow["ID"].ToString(),
                                FirstName = objGolferRow["First_Name"].ToString(),
                                LastName = objGolferRow["Last_Name"].ToString(),
                                Email = objGolferRow["Email"].ToString(),

                                Handicap = Convert.ToInt32(GetDBString(objGolferRow, "Current_Handicap")),
                                AdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Adjusted_Handicap")),
                                Wins = Convert.ToInt32(GetDBString(objGolferRow, "Wins")),
                                SeniorHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Current_Handicap")),
                                SeniorAdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Adjusted_Handicap")),
                                SeniorWins = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Wins")),
                                Qualified = GetDBString(objGolferRow, "Qualified"),
                                Active = GetDBString(objGolferRow, "Active"),
                                SeniorQualified = GetDBString(objGolferRow, "Senior_Qualified"),
                                LastWin = Convert.ToInt32(GetDBString(objGolferRow, "Last_Win_Year")),
                                LastSeniorWin = Convert.ToInt32(GetDBString(objGolferRow, "Last_Senior_Win_Year"))
                            };

                            return Golfer;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                             
                return null;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }
        }

        public IEnumerable<GolferViewModel> DisplayGolfers()
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                // Generate SQL Statement
                //strSQL = "SELECT * FROM Tournaments WHERE ID = " + this.SQLValuePrep("9acb8b84-e459-4dc6-8a80-5e10c79d1c76");
                strSQL = "SELECT * FROM Golfers ORDER BY Last_Name ASC";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<GolferViewModel>();

                foreach (DataRow objGolferRow in objDS.Tables[0].Rows)
                {
                    var Golfer = new GolferViewModel
                    {
                        Id = objGolferRow["Id"].ToString(),
                        FirstName = objGolferRow["First_Name"].ToString(),
                        LastName = objGolferRow["Last_Name"].ToString(),

                        Handicap = Convert.ToInt32(GetDBString(objGolferRow, "Current_Handicap")),
                        AdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Adjusted_Handicap")),
                        Wins = Convert.ToInt32(GetDBString(objGolferRow, "Wins")),
                        SeniorHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Current_Handicap")),
                        SeniorAdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Adjusted_Handicap")),
                        SeniorWins = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Wins")),
                        Qualified = GetDBString(objGolferRow, "Qualified"),
                        Active = GetDBString(objGolferRow, "Active"),
                        SeniorQualified =GetDBString(objGolferRow, "Senior_Qualified")
                                              
                    };

                    result.Add(Golfer);

                }
                return result;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }

        }

        public IEnumerable<GolferViewModel> DisplayActiveGolfers()
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                // Generate SQL Statement
                strSQL = "SELECT * FROM Golfers WHERE Active = '1' ORDER BY Last_Name ASC";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<GolferViewModel>();

                foreach (DataRow objGolferRow in objDS.Tables[0].Rows)
                {
                    var Golfer = new GolferViewModel
                    {
                        Id = objGolferRow["Id"].ToString(),
                        FirstName = objGolferRow["First_Name"].ToString(),
                        LastName = objGolferRow["Last_Name"].ToString(),

                        Handicap = Convert.ToInt32(GetDBString(objGolferRow, "Current_Handicap")),
                        AdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Adjusted_Handicap")),
                        Wins = Convert.ToInt32(GetDBString(objGolferRow, "Wins")),
                        SeniorHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Current_Handicap")),
                        SeniorAdjustedHandicap = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Adjusted_Handicap")),
                        SeniorWins = Convert.ToInt32(GetDBString(objGolferRow, "Senior_Wins")),
                        Qualified = GetDBString(objGolferRow, "Qualified"),
                        Active = GetDBString(objGolferRow, "Active"),
                        SeniorQualified = GetDBString(objGolferRow, "Senior_Qualified")

                    };

                    result.Add(Golfer);

                }
                return result;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }

        }

        public string UpdateGolferInDB(GolferViewModel objGolfer)
        {
            string strSQL = string.Empty;
            System.Guid strId;

            if (objGolfer != null)
            {

               
                    // EDIT vs. CREATE
                    if (string.IsNullOrWhiteSpace(objGolfer.Id))
                    {
                        // Generate a unique GUID ID for golfer
                        strId = System.Guid.NewGuid();
                                               
                        // Generate your Insert SQL Statement
                        strSQL = "INSERT INTO Golfers (Id, First_Name, Last_Name, Current_Handicap, Adjusted_Handicap, Last_Win_Year, Last_Senior_Win_Year, Qualified, Senior_Qualified, Senior_Current_Handicap, Senior_Adjusted_Handicap, Wins, Senior_Wins, Active) VALUES (" + SQLValuePrep(strId.ToString()) + ", " + SQLValuePrep(objGolfer.FirstName) + ", " + SQLValuePrep(objGolfer.LastName) + ", " + SQLValuePrep(objGolfer.Handicap.ToString()) + ", " + SQLValuePrep(objGolfer.AdjustedHandicap.ToString()) + ", " + SQLValuePrep(objGolfer.Qualified) + ", " + SQLValuePrep(objGolfer.LastWin.ToString()) + ", " + SQLValuePrep(objGolfer.LastSeniorWin.ToString()) + ", " + SQLValuePrep(objGolfer.SeniorQualified.ToString()) + ", " + SQLValuePrep(objGolfer.SeniorAdjustedHandicap.ToString()) + ", " + SQLValuePrep(objGolfer.SeniorHandicap.ToString()) + ", " + SQLValuePrep(objGolfer.Wins.ToString()) + ", " + SQLValuePrep(objGolfer.SeniorWins.ToString()) + ", " + SQLValuePrep(objGolfer.Active) + ")";
                                                                }
                    else
                    {
                        strSQL = "UPDATE Golfers SET First_Name = " + SQLValuePrep(objGolfer.FirstName) + ", Last_Name = " + SQLValuePrep(objGolfer.LastName) + ", Current_Handicap = " + SQLValuePrep(objGolfer.Handicap.ToString()) + ", Adjusted_Handicap = " + SQLValuePrep(objGolfer.AdjustedHandicap.ToString()) + ", Last_Win_Year = " + SQLValuePrep(objGolfer.LastWin.ToString()) + ", Last_Senior_Win_Year = " + SQLValuePrep(objGolfer.LastSeniorWin.ToString()) + ", Qualified = " + SQLValuePrep(objGolfer.Qualified.ToString()) + ", Senior_Qualified = " + SQLValuePrep(objGolfer.SeniorQualified.ToString()) + ", Senior_Adjusted_Handicap = " + SQLValuePrep(objGolfer.SeniorAdjustedHandicap.ToString()) + ", Senior_Current_Handicap = " + SQLValuePrep(objGolfer.SeniorHandicap.ToString()) + ", Wins = " + SQLValuePrep(objGolfer.Wins.ToString()) + ", Senior_Wins = " + SQLValuePrep(objGolfer.SeniorWins.ToString()) + ", Active = " + SQLValuePrep(objGolfer.Active.ToString()) + " " + "WHERE Id=" + SQLValuePrep(objGolfer.Id.ToString());
                                              
                    }

                    // Run the SQL and Return if it's successful or not
                    if (DoQuickUpdateQuery(strSQL) == 1)
                    {
                        return "Success!";
                    }
                    else
                    {
                        return "Error!";
                    }
                
                            }
            else
            {
                return "Error. Missing Tournament";
            }
        }

        public GolferViewModel CalculateNewHandicap(GolferViewModel objGolfer)
        {
            if (objGolfer != null)
            {
                return objGolfer;
            }
            else
            {
                return objGolfer;
            }
        }

        #endregion

        #region Courses Signatures

        public IEnumerable<CourseViewModel> DisplayCourses()
        {
            System.Data.DataSet objDS = null;
            string strSQL = "";

            try
            {
                // Generate SQL Statement
                strSQL = "SELECT * FROM Courses ORDER BY Display_Name ASC";

                objDS = DoQuickSelectQuery(strSQL);

                var result = new List<CourseViewModel>();

                foreach (DataRow objGolferRow in objDS.Tables[0].Rows)
                {
                    var Course = new CourseViewModel
                    {
                        Id = objGolferRow["Id"].ToString(),
                        DisplayName = objGolferRow["Display_Name"].ToString(),
                        Slope = objGolferRow["Course_Slope"].ToString(),
                        Rating = objGolferRow["Course_Rating"].ToString(),
                        Par = objGolferRow["Course_Par"].ToString()
                                       };

                    result.Add(Course);

                }
                return result;
            }
            finally
            {
                this.DestroyDataSet(objDS);
            }

        }

        #endregion

        #region Helper Signatures
        
        #region Create

        public class SessionHelper
        {
            public enum StringValues
            {
                Unknown1,
                Unknown2
            }

           
            //public static string GetSessionStateString(StringValues Key)
            //{

            //    object objValue = null;

            //    objValue = System.Web.HttpContext.Current.Session[Key.ToString()];
            //    if (objValue != null ++ objValue is string)
            //    {
            //        return Convert.ToString(objValue);
            //    }
            //    else
            //    {
            //        return string.Empty;
            //    }

            //}

            public static void SetSessionStateString(StringValues Key, string Value)
            {
                if (string.IsNullOrEmpty(Value))
                {
                    System.Web.HttpContext.Current.Session.Remove(Key.ToString());
                }
                else
                {
                    System.Web.HttpContext.Current.Session[Key.ToString()] = Value;
                }

            }
        }

       

       

        private System.Data.IDbConnection OpenDBConnection()
        {
            System.Data.IDbConnection objSQLConnection = null;
            string strConnection = string.Empty;

            // Create new DB Connection
            objSQLConnection = new System.Data.SqlClient.SqlConnection();


            // SQL Server DEV BOX Connection String
            strConnection = "Data Source=DEV44;Initial Catalog=FJF_GOLF_OUTING;Persist Security Info=True;User ID=sa;Password=DFSAdmin1";

           // objSQLConnection.ConnectionString = "Data Source=" + GetApplicationSetting("SQLServerMachineName") + ";Initial Catalog=" + GetApplicationSetting("SQLServerDBName") + ";Persist Security Info=True;User ID=" + GetApplicationSetting("SQLServerUserId") + ";Password=" + GetApplicationSetting("SQLServerPW");

            //// SQL Express Laptop Connection String
            //{
            //    strConnection = "Server=" + ".\\SQLEXPRESS" + ";Initial Catalog=" + GetApplicationSetting("SQLServerDBName") + ";Integrated Security=SSPI;";
            //    objSQLConnection.ConnectionString = strConnection;
            //}



            //strConnection = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=D:\\DBs\\FJF_Memorial_Golf_Outing.mdf;Integrated Security=True;Connect Timeout=30";
            objSQLConnection.ConnectionString = strConnection;
            objSQLConnection.Open();

            return objSQLConnection;

        }

        private System.Data.IDbCommand CreateDBCommand(System.Data.IDbConnection vobjDBConnection)
        {
            System.Data.IDbCommand objDBCommand = null;

            objDBCommand = vobjDBConnection.CreateCommand();
            objDBCommand.CommandType = System.Data.CommandType.Text;

            return objDBCommand;
        }

        private System.Data.DataSet OpenDataSet(System.Data.IDbConnection vobjDBConnection, string vstrSQL)
        {
            System.Data.IDbCommand objCommand = null;
            System.Data.DataSet objDataSet = null;
            System.Data.IDataAdapter objDataAdapter = null;


            try
            {
                // Create Command
                objCommand = CreateDBCommand(vobjDBConnection);
                objCommand.CommandText = vstrSQL;

                objDataAdapter = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)objCommand);

                // Create new dataset
                objDataSet = new System.Data.DataSet();

                objDataAdapter.Fill(objDataSet);

                return objDataSet;

            }
            finally
            {
                // Destroy the Data Adapter
                DestroyDataAdapter(objDataAdapter);

                // Close the Data Reader
                DestroyDBCommand(objCommand);

            }
        }

        private System.Data.IDataReader OpenSQLDataReader(System.Data.IDbConnection vobjDBConnection, string vstrSQL)
        {
            System.Data.IDbCommand objCommand = null;
            System.Data.IDataReader objDataReader = null;

            try
            {
                objCommand = CreateDBCommand(vobjDBConnection);
                objCommand.CommandText = vstrSQL;

                objDataReader = objCommand.ExecuteReader();

                return objDataReader;
            }
            finally
            {
                // Close the Data Reader
                DestroyDBCommand(objCommand);
            }
        }

        public System.Data.DataSet DoQuickSelectQuery(string vstrSQL)
        {
            System.Data.IDbConnection objDBConn = null;
            System.Data.DataSet objResultDataSet = null;

            try
            {
                // Open DB
                objDBConn = OpenDBConnection();

                // Execute some sql
                objResultDataSet = OpenDataSet(objDBConn, vstrSQL);

                // Return the Resulting DataSet
                return objResultDataSet;

            }
            catch (Exception ex)
            {

                // Something went wrong
                if (objResultDataSet != null)
                {
                    DestroyDataSet(objResultDataSet);
                }

                return null;
            }
            finally
            {
                // Close DB Connection
                CloseDBConnection(objDBConn);
            }

        }

        public int DoQuickUpdateQuery(string vstrSQL)
        {

            IDbConnection objDBConn = null;
            IDbCommand objDBCommand = null;
            int intRowsAffected = -1;


            try
            {
                // Open DB
                objDBConn = OpenDBConnection();

                // Create a Command
                objDBCommand = CreateDBCommand(objDBConn);

                // Set the SQL
                objDBCommand.CommandText = vstrSQL;

                // Execute the sql, which should reflect the number of rows that this update statement affected
                intRowsAffected = objDBCommand.ExecuteNonQuery();

                // Return the Resulting DataSet
                return intRowsAffected;

            }
            finally
            {
                // Destroy Command
                DestroyDBCommand(objDBCommand);

                // Close DB Connection
                CloseDBConnection(objDBConn);

            }

        }

        #endregion
        
        #region Destroy

        private void CloseDBConnection(System.Data.IDbConnection roConnection)
        {
            if (roConnection != null)
            {
                roConnection.Close();
                roConnection.Dispose();
                roConnection = null;
            }
        }

        private void DestroyDBCommand(System.Data.IDbCommand robjDBCommand)
        {
            if (robjDBCommand != null)
            {
                robjDBCommand.Dispose();
                robjDBCommand = null;
            }
        }

        public void DestroyDataSet(System.Data.DataSet DS)
        {
            if (DS != null)
            {
                DS.Dispose();
                DS = null;
            }
        }

        public void DestroyDataAdapter(System.Data.IDataAdapter robjDataAdapter)
        {
            if (robjDataAdapter != null)
            {
                if (robjDataAdapter is System.Data.SqlClient.SqlDataAdapter == true)
                {
                    ((System.Data.SqlClient.SqlDataAdapter)robjDataAdapter).Dispose();
                }
                robjDataAdapter = null;
            }
        }

        public void DestroyDataReader(System.Data.IDataReader robjDataReader)
        {
            if (robjDataReader != null)
            {
                // Close the Reader, if Necessary
                if (robjDataReader.IsClosed == false)
                {
                    robjDataReader.Close();
                }

                // Dispose of the DataReader
                robjDataReader.Dispose();

                robjDataReader = null;
            }
        }

        #endregion
        
        #region SQL Prep

        public string SQLValuePrep(string vData)
        {
            //Pad all single quotes with additional single quote
            if (vData == null)
            {
                return "NULL";

            }
            else if (vData.Length == 0)
            {
                return "NULL";
            }
            else
            {
                //Replace all single quotes with ''quotes
                vData = vData.Replace("'", "''");

                //Encase data in single quotes
                vData = "'" + vData + "'";

                return vData;
            }
        }

        public String SQLDatePrep(DateTime vData)
        {
            //Pad all single quotes with additional single quote
            if (vData == null)
            {
                return DateTime.Now.ToShortDateString();
             }
            else
            {
                             
                return vData.ToShortDateString();
            }
        }
       
        #endregion

        public string GetDBString(System.Data.DataRow Row, string FieldName)
        {

            string strValue = "";


            if (Row != null)
            {
                if (Row.IsNull(FieldName) == false)
                {
                    strValue = Convert.ToString(Row[FieldName]);
                }
                else
                {
                    strValue = "0";
                }

            }

            return strValue;

        }

        public String GetDate(System.Data.DataRow Row, string FieldName)
        {
            DateTime objDate;
           
            if (Row != null)
            {
                if (Row.IsNull(FieldName) == false)
                {
                    objDate = (DateTime)Row[FieldName];
                    return objDate.ToShortDateString();
                }
                else
                {
                    return DateTime.Now.ToShortDateString();
                }
            }
            else
            {
                return DateTime.Now.ToShortDateString();
            }

        }

        private System.Data.DataSet GetBaseDataSet()
        {

            System.Data.DataSet objDS = null;

            objDS = new System.Data.DataSet();

            objDS.Tables.Add(new System.Data.DataTable());

            var _with1 = objDS.Tables[0].Columns;
            _with1.Add(new System.Data.DataColumn("ID", typeof(System.String)));
            _with1.Add(new System.Data.DataColumn("Display_Name", typeof(System.String)));
            _with1.Add(new System.Data.DataColumn("Course_Slope", typeof(System.Int32)));
            _with1.Add(new System.Data.DataColumn("Course_Rating", typeof(System.Decimal)));

            return objDS;

        }

        public decimal GetDBDecimal(System.Data.DataRow vobjDataRow, string FieldName, decimal vdecDefaultNULLValue = 0m)
        {
            if (vobjDataRow.IsNull(FieldName) == true)
            {
                return vdecDefaultNULLValue;
            }
            else
            {
                return Convert.ToDecimal(vobjDataRow[FieldName]);
            }
        }

        public int GetDBInteger(System.Data.DataRow vobjDataRow, string FieldName, int vintDefaultNULLValue = 0)
        {
            if (vobjDataRow.IsNull(FieldName) == true)
            {
                return vintDefaultNULLValue;
            }
            else
            {
                return Convert.ToInt32(vobjDataRow[FieldName]);
            }
        }


        #endregion
        
    }

    public class GolferTournamentViewModel
    {
        public GolferViewModel Golfer { get; set; }
        public TournamentViewModel Tournament { get; set; }
        public IEnumerable<GolferViewModel> Golfers { get; set; }
        public IEnumerable<LeaderboardViewModel> Rounds { get; set; }
        
    }
   
    public class TournamentViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Champion { get; set; }
        public string SeniorChampion { get; set; }
        public string ScrambleChampions { get; set; }
        public string Round1_Date { get; set; }
        public string Round2_Date { get; set; }
        public string Round3_Date { get; set; }
        public string Round4_Date { get; set; }
        public string Round1_Course { get; set; }
        public string Round2_Course { get; set; }
        public string Round3_Course { get; set; }
        public string Round4_Course { get; set; }
        public int CourseStatus { get; set; }
    }

    public class LeaderboardViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GolferId { get; set; }
        public string TournamentName { get; set; }
        public int Handicap { get; set; }
        public int Adusted_Handicap { get; set; }
        public int Sr_Handicap { get; set; }
        public int Total_Net { get; set; }

        public int Round1_Gross { get; set; }
        public int Round2_Gross { get; set; }
        public int Round3_Gross { get; set; }
        public int Round4_Gross { get; set; }

        public int Round1_Net { get; set; }
        public int Round2_Net { get; set; } 
        public int Round3_Net { get; set; }
        public int Round4_Net { get; set; }

        public decimal Round1_Hcp_Index { get; set; }
        public decimal Round2_Hcp_Index { get; set; }
        public decimal Round3_Hcp_Index { get; set; }
        public decimal Round4_Hcp_Index { get; set; }

        public int Round1_SrNet { get; set; }
        public int Round2_SrNet { get; set; }
        public int Round3_SrNet { get; set; }
        public int Round4_SrNet { get; set; }

        //// Add to DB
        //public decimal Round1_SrHcp_Index { get; set; }
        //public decimal Round2_SrHcp_Index { get; set; }
        //public decimal Round3_SrHcp_Index { get; set; }
        //public decimal Round4_SrHcp_Index { get; set; }

       
    }

    public class RoundViewModel
    {
        public string Id { get; set; }
        public string GolferId { get; set; }
        public int Rd_Hcp { get; set; }
        public int Rd_SrHcp { get; set; }
        public string Tournament_Name { get; set; }

        public int Rd1_Score_Gross { get; set; }
        public int Rd2_Score_Gross { get; set; }
        public int Rd3_Score_Gross { get; set; }
        public int Rd4_Score_Gross { get; set; }

        public int Rd1_Score_Net { get; set; }
        public int Rd2_Score_Net { get; set; }
        public int Rd3_Score_Net { get; set; }
        public int Rd4_Score_Net { get; set; }

        public decimal Rd1_Hcp_Index { get; set; }
        public decimal Rd2_Hcp_Index { get; set; }
        public decimal Rd3_Hcp_Index { get; set; }
        public decimal Rd4_Hcp_Index { get; set; }

        public string Round1_Date { get; set; }
        public string Round2_Date { get; set; }
        public string Round3_Date { get; set; }
        public string Round4_Date { get; set; }    
    }

    public class GolferViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Handicap { get; set; }

        public int AdjustedHandicap { get; set; }
        public int Wins { get; set; }
        public string Qualified { get; set; }
        public string SeniorQualified { get; set; }
        public int SeniorHandicap { get; set; }
        public int SeniorAdjustedHandicap { get; set; }
        public int SeniorWins { get; set; }
        public string Active { get; set; }
        public int LastWin { get; set; }
        public int LastSeniorWin { get; set; }

    }

    public class TournamentPlayerViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Handicap { get; set; }
        public int AdjustedHandicap { get; set; }
        public int Wins { get; set; }
        public string Qualified { get; set; }
        public string SeniorQualified { get; set; }
        public int SeniorHandicap { get; set; }
        public int SeniorAdjustedHandicap { get; set; }
        public int SeniorWins { get; set; }
        public string Active { get; set; }
        public int LastWin { get; set; }
        public int LastSeniorWin { get; set; }

    }

    public class CourseViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Slope { get; set; }
        public string Rating { get; set; }
        public string Par { get; set; }

    }
       

       

     











    }
