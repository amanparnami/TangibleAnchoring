using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring
{
    public class Criteria 
    {
       
        private Dictionary<string, List<string>> quesIdAnsIdsMap;

	public Dictionary<string, List<string>> QuesIdAnsIdsMap
	{
		get { return quesIdAnsIdsMap;}
		set { quesIdAnsIdsMap = value;}
	}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="questionId">Question questionId to be checked</param>
        /// <param name="csvAnswerIds">Comma separated answer ids</param>
        public Criteria(string questionId, string csvAnswerIds)
        {
            Dictionary<string, List<string>> tempDict = new Dictionary<string,List<string>>();
            string keyQuesId =  questionId;

            if (csvAnswerIds.StartsWith(",")) //starting with a comma
            {
                csvAnswerIds = csvAnswerIds.Substring(1).Trim();
            }
            else if (csvAnswerIds.EndsWith(",")) //ending with a comma
            {
                csvAnswerIds = csvAnswerIds.TrimEnd(',').Trim();
            }

            int numAnswerIds = csvAnswerIds.Split(',').Length;
            List<string> valueAnsIds = new List<string>();

            if (numAnswerIds == 1) //single element or empty string to be added
            {
                if (csvAnswerIds.Length > 0) //single element
                {
                    valueAnsIds.Add(csvAnswerIds);
                }
                else //empty string
                {
                    //Initialize with All Answers
                    valueAnsIds.Add("All Answers");
                }
            }
            else //multiple elements to be added
            {
                string[] answerIdArr = csvAnswerIds.Split(',');
                for (int i = 0; i < numAnswerIds; i++)
                {
                    valueAnsIds.Add(answerIdArr[i].Trim());
                }
            }
            tempDict.Add(keyQuesId, valueAnsIds);
            quesIdAnsIdsMap = tempDict;
        }

        /// <summary>
        /// Add ids to criteria
        /// </summary>
        /// <param name="questionId">Question questionId to be checked</param>
        /// <param name="csvAnswerIds">Comma separated answer ids</param>
        /// <returns>0 if "no change" -1 if "fails" 1 if succeeds</returns>
        public int AddIds(string questionId, string csvAnswerIds)
        {
            string keyQuesId = questionId;

            if (csvAnswerIds.StartsWith(",")) //starting with a comma
            {
                csvAnswerIds = csvAnswerIds.Substring(1).Trim();
            }
            else if (csvAnswerIds.EndsWith(",")) //ending with a comma
            {
                csvAnswerIds = csvAnswerIds.TrimEnd(',').Trim();
            }

            if (quesIdAnsIdsMap.ContainsKey(keyQuesId)) //entry for the question already exists
            {
                if (quesIdAnsIdsMap[keyQuesId][0] == "All Answers") //all values exist then replace
                {
                    int numAnswerIds = csvAnswerIds.Split(',').Length;
                    if (numAnswerIds == 1) //single element or empty string
                    {
                        if (csvAnswerIds.Length > 0) //single element
                        {
                            quesIdAnsIdsMap[keyQuesId].Clear();
                            quesIdAnsIdsMap[keyQuesId].Add(csvAnswerIds);
                        }
                        else //empty string
                        {
                           //Do nothing
                            return 0; // no changes made
                        }
                    }
                    else //multiple elements
                    {
                        string[] answerIdArr = csvAnswerIds.Split(',');
                        //First remove the existing element "All Answers" then start adding new ones
                        quesIdAnsIdsMap[keyQuesId].Clear();
                        for (int i = 0; i < numAnswerIds; i++)
                        {
                            quesIdAnsIdsMap[keyQuesId].Add(answerIdArr[i].Trim());
                        }
                    }
                }
                else //check if the values to be added exist
                {
                    int numAnswerIds = csvAnswerIds.Split(',').Length;
                    if (numAnswerIds == 1) //single element or empty string to be added
                    {
                        if (csvAnswerIds.Length > 0) //single element
                        {
                            if (csvAnswerIds == "All Answers")
                            {
                                quesIdAnsIdsMap[keyQuesId].Clear();
                                quesIdAnsIdsMap[keyQuesId].Add(csvAnswerIds);
                            }
                            else
                            {
                                if (!quesIdAnsIdsMap[keyQuesId].Contains(csvAnswerIds))
                                {
                                    quesIdAnsIdsMap[keyQuesId].Add(csvAnswerIds);
                                }
                            }
                        }
                        else //empty string
                        {
                            //Do nothing
                            return 0;
                        }
                    }
                    else //muliple elements to be added
                    {
                        string[] answerIdArr = csvAnswerIds.Split(',');
                        for (int i = 0; i < numAnswerIds; i++)
                        {
                            if (!quesIdAnsIdsMap[keyQuesId].Contains(answerIdArr[i])) //add only if element doesn't exist
                            {
                                quesIdAnsIdsMap[keyQuesId].Add(answerIdArr[i].Trim());
                            }
                        }
                    }
                }
                return 1;
            }
            else //create a new entry for this question
            {
                int numAnswerIds = csvAnswerIds.Split(',').Length;
                List<string> valueAnsIds = new List<string>();

                if (numAnswerIds == 1) //single element or empty string to be added
                {
                    if (csvAnswerIds.Length > 0) //single element
                    {
                        valueAnsIds.Add(csvAnswerIds);
                    }
                    else //empty string
                    {
                        //Initialize with All Answers
                        valueAnsIds.Add("All Answers");
                    }
                }
                else //multiple elements to be added
                {
                    string[] answerIdArr = csvAnswerIds.Split(',');
                    for (int i = 0; i < numAnswerIds; i++)
                    {
                        valueAnsIds.Add(answerIdArr[i].Trim());
                    }
                }
                quesIdAnsIdsMap.Add(keyQuesId, valueAnsIds);
                return 1;
            }
        }

        /// <summary>
        /// Remove Ids from criteria
        /// </summary>
        /// <param name="questionId">Question questionId to be checked</param>
        /// <param name="csvAnswerIds">Comma separated answer ids to be removed</param>
        /// <returns>0 if "no change" -1 if "fails" 1 if succeeds</returns>
        public int RemoveIds(string questionId, string csvAnswerIds)
        {
            string keyQuesId = questionId;

            if (csvAnswerIds.StartsWith(",")) //starting with a comma
            {
                csvAnswerIds = csvAnswerIds.Substring(1).Trim();
            }
            else if (csvAnswerIds.EndsWith(",")) //ending with a comma
            {
                csvAnswerIds = csvAnswerIds.TrimEnd(',').Trim();
            }

            if (quesIdAnsIdsMap.ContainsKey(keyQuesId)) //entry for the question exists
            {
                if (quesIdAnsIdsMap[keyQuesId][0] == "All Answers") //all values exist then replace
                {
                    //Nothing to be done
                    return 0;
                }
                else //check if the values to be added exist
                {
                    int numAnswerIds = csvAnswerIds.Split(',').Length;
                    if (numAnswerIds == 1) //single element or empty string to be removed
                    {
                        if (csvAnswerIds.Length > 0) //single element
                        {
                            if (csvAnswerIds == "All Answers")
                            {
                                //Nothing to be done
                                return 0;
                            }
                            else
                            {
                                if (quesIdAnsIdsMap[keyQuesId].Contains(csvAnswerIds))
                                {
                                    //Remove the element whose match is found
                                    quesIdAnsIdsMap[keyQuesId].Remove(csvAnswerIds);
                                }
                            }
                        }
                        else //empty string
                        {
                            //Do nothing
                            return 0;
                        }
                    }
                    else //muliple elements to be removed
                    {
                        string[] answerIdArr = csvAnswerIds.Split(',');
                        for (int i = 0; i < numAnswerIds; i++)
                        {
                            if (quesIdAnsIdsMap[keyQuesId].Contains(answerIdArr[i].Trim())) //remove only if element does exist
                            {
                                //Remove the element whose match is found
                                quesIdAnsIdsMap[keyQuesId].Remove(answerIdArr[i].Trim());
                            }
                        }
                    }
                }

                //if all the elements for a questionId have been removed then fill with "All Answers"
                if (quesIdAnsIdsMap[keyQuesId].Count() == 0)
                {
                    quesIdAnsIdsMap[keyQuesId].Add("All Answers");
                }
                return 1;
            }
            else //entry for question doesn't exists
            {
                //Nothing to be done
                return 1;
            }
        }

        /// <summary>
        /// Check is questionId, answerId pair matches criteria
        /// </summary>
        /// <param name="questionId">Question questionId to be checked</param>
        /// <param name="csvAnswerIds">Comma separated answer ids</param>
        /// <returns>0 if "no change" -1 if "fails" 1 if succeeds</returns>
        public bool Check(string questionId, string answerId)
        {
            if (quesIdAnsIdsMap.ContainsKey(questionId))
            {
                if (quesIdAnsIdsMap[questionId].Contains(answerId) || quesIdAnsIdsMap[questionId][0] == "All Answers") return true;
            }
            return false;
        }

        public string ToLogString()
        {
            string returnString = "";

            returnString = string.Join(";", quesIdAnsIdsMap.Select(x => x.Key + "=" + string.Join(",", x.Value.ToArray())));

            return returnString;
        }
    }
}
