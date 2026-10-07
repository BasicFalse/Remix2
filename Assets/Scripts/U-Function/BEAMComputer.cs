using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UFunction
{
    public class BEAMComputer : MonoBehaviour
    {
        public enum ComputerState
        {
            Sleep = 0,
            Search = 1,
            Questions = 3,
            Beams = 4,
            Assessment = 5
        }

        public static BEAMComputer instance { get; private set; }

        public TextMeshPro screenText;

        public TextMeshPro functionSelectText;

        public TextMeshPro statusScreen;

        public ComputerState currentState;

        public string computerSearchQuery;

        public string computerSearchResult;

        public SearchResults SearchResults;

        public float beamSelectTime;
        public int beamSelect;
        public int beamSelectIt;
        public string beamStatus;

        public int MapAssessmentSelector = 0;
        public int MapAssessmentSelecit = 0;
        public bool asseSelected = false;

        public string spinner;
        public float spinTimer;
        public float screenSpinTime;

        public int questSelect;
        public bool questDisplay;

        private float fracFetchTimeout;
        private bool frac7293fetched;

        private readonly string quest1 = "WHAT ARE BEAMS?";
        private readonly string quest2 = "\n\nWHAT'S A FRACTURE?";
        private readonly string quest3 = "\n\nWHEN WILL THIS SHARD FRACTURE?";
        private readonly string quest4 = "\n\nWHAT IS THIS SHARD?";

        private readonly string mapAssess1 = "\"Volcano\"";
        private readonly string mapAssess2 = "\"Forest\"";
        private readonly string mapAssess3 = "\"Tunnels\"";
        private readonly string mapAssess4 = "\"Pillars\"";
        private readonly string mapAssess5 = "\"Mountains\"";
        private readonly string mapAssess6 = "\"Canyons\"";
        //private readonly string mapAssess0 = "\"Null\"";

        private readonly string mapAssessComp1 = "MISSING.";
        private readonly string mapAssessComp2 = "Stable; expansion slowing.";
        private readonly string mapAssessComp3 = "Stable; detected fracturing continuing.";
        private readonly string mapAssessComp4 = "Stable; fog appears to have covered the area for the winter season.";
        private readonly string mapAssessComp5 = "Stable; tech retrived, Identified as BEAM-H-605.";
        private readonly string mapAssessComp6 = "Stable.";


        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            currentState = ComputerState.Sleep;
            UpdateScreen();

        }

        public void Update()
        {
            LiveEventResponce();
            SpinnerAnim();
            //if (beamRoom == isActiveAndEnabled)
            //{
                UpdateScreen();
            //}
            if(questSelect == 3 && questDisplay)
            {
                if(fracFetchTimeout < 10 && !frac7293fetched)
                {
                    fracFetchTimeout += .1f;
                }
                else
                {
                    frac7293fetched = true;
                }
            }
        }


        public void PressButton(KeyboardButton buttonPressed)
        {
            switch (currentState)
            {
                case ComputerState.Sleep:
                    ProcessSleepState(buttonPressed);
                    break;
                case ComputerState.Search:
                    ProcessSearchState(buttonPressed, SearchResults);
                    break;
                case ComputerState.Questions:
                    ProcessQuestionsState(buttonPressed);
                    break;
                case ComputerState.Beams:
                    ProcessBeamsState(buttonPressed);
                    break;
                    case ComputerState.Assessment:
                    ProcessAssessmentState(buttonPressed);
                    break;
            }
        }

        private void SpinnerAnim()
        {
            spinTimer = spinTimer - 0.1f;
            if (spinTimer <= 0f)
            {
                spinTimer = 2f;
                switch (spinner)
                {
                    case ("-"):
                        spinner = "\\";
                        break;
                    case ("\\"):
                        spinner = "|";
                        break;
                    case ("|"):
                        spinner = "/";
                        break;
                    case ("/"):
                        spinner = "-";
                        break;
                    default:
                        spinner = "-";
                        break;
                }
            }
        }



        private string GetSearchResult(SearchResults searchResults, string query)
        {
            foreach (var result in searchResults.Results)
            {
                if (result.Querys.Contains(query))
                {
                    return result.Result != null ? result.Result.text : "No result available.";
                }
            }
            return "No result available.";
        }
        private string ValidateSearchQuery(SearchResults searchResults, string query)
        {
            if (searchResults != null)
            {
                foreach (var result in searchResults.Results)
                {
                    if (result.Querys.Contains(query))
                    {
                        return "Valid";
                    }
                }
            }
            return "Invalid";
        }


        private void SwitchToSearchState()
        {
            currentState = ComputerState.Search;
            UpdateScreen();
        }
        private void SwitchToQuestionsState()
        {
            currentState = ComputerState.Questions;
            UpdateScreen();
        }
        private void SwitchToBeamsState()
        {
            currentState = ComputerState.Beams;
            UpdateScreen();
        }
        private void SwitchToAssessmentState()
        {
            currentState = ComputerState.Assessment;
            UpdateScreen();
        }

        private void LiveEventResponce()
        {
            if (LiveEventManager.instance.inSession)
            {
                if (LiveEventManager.instance.isPlaying)
                {
                    statusScreen.text = "U-STABILITY: 95%\r\n\r\n-------------------\r\n\r\n WARNING\nGEOLOGICAL MOVEMENT DETECTED\r\n\r\n-------------------\r\n\r\nU-NET CONNECTIVITY STABLE";
                }
                else
                {
                    statusScreen.text = "U-STABILITY: 95%\r\n\r\n-------------------\r\n\r\n WARNING\nMINOR INSTABILITIES DETECTED\r\n\r\n-------------------\r\n\r\nU-NET CONNECTIVITY STABLE";

                }
            }else{statusScreen.text = "U-STABILITY: 95%\r\n\r\n-------------------\r\n\r\nU-NET CONNECTIVITY STABLE";}
        }


        public void ProcessSleepState(KeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "goUp":
                    SwitchToQuestionsState();
                    break;
                case "goDown":
                    SwitchToQuestionsState();
                    break;
            }
            UpdateScreen();
        }
        public void ProcessSearchState(KeyboardButton buttonPressed, SearchResults searchResults)
        {
            switch (buttonPressed.characterString)
            {
                case "goUp":
                    SwitchToAssessmentState();
                    break;
                case "goDown":
                    SwitchToQuestionsState();
                    break;
                default:
                    if (computerSearchQuery.Length < 50)
                    {
                        computerSearchResult = "";
                        computerSearchQuery += buttonPressed.characterString;
                    }
                    break;
                case "DELETE":
                    if (computerSearchQuery.Length > 0)
                    {
                        computerSearchQuery = computerSearchQuery[..^1];
                    }
                    break;
                case "ENTER":
                    if (!string.IsNullOrEmpty(computerSearchQuery))
                    {
                        computerSearchResult = "";

                        string validationResult = ValidateSearchQuery(searchResults, computerSearchQuery);
                        if (validationResult == "Valid")
                        {
                            computerSearchResult = GetSearchResult(searchResults, computerSearchQuery);
                        }
                        else if (searchResults != null)
                        {
                            computerSearchResult = "ERROR:400";
                        }
                        else
                        {
                            computerSearchResult = "ERROR: UNAUTHORIZED";
                        }
                    }
                    break;
                case "ESCAPE":
                    computerSearchResult = "";
                    break;
                case "SPACE":
                    if (computerSearchQuery.Length < 50)
                    {
                        computerSearchQuery += " ";
                    }
                    break;
            }
            UpdateScreen();
        }
        public void ProcessQuestionsState(KeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "goUp":
                    SwitchToSearchState();
                    break;
                case "goDown":
                    SwitchToBeamsState();
                    break;
                case "1":
                    if (!questDisplay) { questSelect = 1; }
                    break;
                case "2":
                    if (!questDisplay) { questSelect = 2; }
                    break;
                case "3":
                    if (!questDisplay) { questSelect = 3; }
                    break;
                case "4":
                    if (!questDisplay) { questSelect = 4; }
                    break;
                case "ENTER":
                    if(questSelect != 0) { questDisplay = true; }
                    break;
                case "DELETE":
                    questDisplay = false;
                    questSelect = 0;
                    break;
            }
            UpdateScreen();
        }
        public void ProcessBeamsState(KeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "goUp":
                    SwitchToQuestionsState();
                    break;
                case "goDown":
                    SwitchToAssessmentState();
                    break;
                //case "1":
                //    beamSelect = 1;
                //    break;
                case "ENTER":
                    beamSelectIt = beamSelect;
                    beamSelectTime = 2f;
                    break;
                default:
                    beamSelect = Convert.ToInt32(buttonPressed.characterString);
                    break;
            }
            
            UpdateScreen();
        }
        public void ProcessAssessmentState(KeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "goUp":
                    asseSelected =false;
                    SwitchToBeamsState();
                    break;
                case "goDown":
                    asseSelected=false;
                    SwitchToSearchState();
                    break;
                case "DELETE":
                    asseSelected = false;
                    break;
                case "ENTER":
                    MapAssessmentSelecit = MapAssessmentSelector;
                    asseSelected = true;
                    break;
                default:
                    MapAssessmentSelector = Convert.ToInt32(buttonPressed.characterString);
                    asseSelected = false;
                    break;
            }
            
            UpdateScreen();
        }

        public void UpdateVeiwedBeamStats(int beamSele)
        {

        }

        private void UpdateScreen()
        {

            UpdateFunctionText();
            switch (currentState)
            {
                
                case ComputerState.Sleep:
                    screenText.text = "...";
                    break;
                case ComputerState.Search:
                    screenText.text = "QUERY:" + computerSearchQuery + "\n" + computerSearchResult;
                    break;
                case ComputerState.Questions:
                    if (questDisplay)
                    {
                        switch (questSelect)
                        {
                            case 1:
                                screenText.text = "EVERY SHARDS GUARD AND CHIVALRY, BEAMS OVERSEE THINGS THAT MAY GET TOO OUT OF HAND AND IN CERTAIN CIRCUMSTANCES A HEAVY BEAM MAY BE DEPLOYED DURING DANGROUS SITUATIONS.";
                                break;
                            case 2:
                                screenText.text = "A FRACTURE IS AN EVENT WHEN A SHARD IS UNSTABLE AND EITHER \"FRACTURES\" INTO 4 OR MORE SHARDS, OR DISSIPATES FROM THE CLUSTER.";
                                break;
                            case 3:
                                screenText.text = "UNKNOWN.";
                                break;
                            case 4: // "THE SHARD YOU ARE CURRENTLY IS DESIGNATED AS U-849, IT IS ONE OF THE SHARDS PRODUCED AS A RESULT OF U-813 FRACTURING." nonsense
                                screenText.text = "THIS SHARD IS DESIGNATED AS U-849, IT IS A COMPOSITE SHARD PRODUCED BY U-813, U-667, U-731, AND U-809.";
                                break;
                        }


                    }
                    else
                    {
                        if (questSelect == 0) { screenText.text = quest1 + quest2 + quest3+ quest4; }
                        if (questSelect == 1) { screenText.text = quest1 + " <-"  + quest2+ quest3+ quest4; }
                        if (questSelect == 2) { screenText.text = quest1 + quest2 + " <-" + quest3+ quest4; }
                        if (questSelect == 3) { screenText.text = quest1 + quest2 + quest3+ " <-" + quest4; }
                        if (questSelect == 4) { screenText.text = quest1 + quest2 + quest3+ quest4+ " <-"; }
                    }

                    
                    break;
                case ComputerState.Beams:
                    if (beamSelectTime >= 0 && beamSelectIt != 0)
                    {
                        beamSelectTime -= 0.1f;
                        if (beamSelectTime <= 0)
                        {
                            UpdateVeiwedBeamStats(beamSelect);
                            beamSelectIt = 0;
                            beamSelectTime = 5;
                        }
                    }
                    string beam225 = "BEAM-S-225: " + "PATROL";
                    string beamh605 = "BEAM-H-605: " + "SYNCING";
                    screenText.text =
                        beam225 + ((beamSelectIt == 1) ? " " + spinner : " =") + ((beamSelect == 1) ? " <-" : "") + "\n\n" +
                        beamh605 + ((beamSelectIt == 2) ? " " + spinner : " =") + ((beamSelect == 2) ? " <-" : "");
                    break;
                case ComputerState.Assessment:
                    if (asseSelected)
                    {
                        screenText.text = MapAssessmentSelector switch
                        {
                            1 => mapAssessComp1,
                            2 => mapAssessComp2,
                            3 => mapAssessComp3,
                            4 => mapAssessComp4,
                            5 => mapAssessComp5,
                            6 => mapAssessComp6,
                            _ => "ERROR, SELECTION COULD NOT BE PARSED."
                        };
                    }
                    else
                    {
                        string n="\n";
                        string p=" <-";
                        screenText.text = MapAssessmentSelector switch
                        {
                            1 => mapAssess1 + p + n + mapAssess2 + n + mapAssess3 + n + mapAssess4 + n + mapAssess5 + n + mapAssess6,
                            2 => mapAssess1 + n + mapAssess2 + p + n + mapAssess3 + n + mapAssess4 + n + mapAssess5 + n + mapAssess6,
                            3 => mapAssess1 + n + mapAssess2 + n + mapAssess3 + p + n + mapAssess4 + n + mapAssess5 + n + mapAssess6,
                            4 => mapAssess1 + n + mapAssess2 + n + mapAssess3 + n + mapAssess4 + p + n + mapAssess5 + n + mapAssess6,
                            5 => mapAssess1 + n + mapAssess2 + n + mapAssess3 + n + mapAssess4 + n + mapAssess5 + p + n + mapAssess6,
                            6 => mapAssess1 + n + mapAssess2 + n + mapAssess3 + n + mapAssess4 + n + mapAssess5 + n + mapAssess6 + p,
                            _ => mapAssess1 + n + mapAssess2 + n + mapAssess3 + n + mapAssess4 + n + mapAssess5 + n + mapAssess6,
                        };
                    }
                break;
            }
        }
        private void UpdateFunctionText()
        {
            if (currentState == ComputerState.Sleep)
            {
                functionSelectText.text = "|SLEEP";
            }
            else
            {
                functionSelectText.text=
                ((currentState == ComputerState.Search)     ? "|SEARCH"     : "SEARCH")     + "\n\n" +
                ((currentState == ComputerState.Questions)  ? "|QUESTIONS"  : "QUESTIONS")  + "\n\n" +
                ((currentState == ComputerState.Beams)      ? "|BEAMS"      : "BEAMS")      + "\n\n" +
                ((currentState == ComputerState.Assessment) ? "|ASSESSMENT" : "ASSESSMENT")          ;
            }
        }
    }
}
