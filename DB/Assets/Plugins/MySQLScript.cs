using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;     //C#의 데이터 테이블 때문에 사용
using MySql.Data;     //MYSQL함수들을 불러오기 위해서 사용

using MySql.Data.MySqlClient;
using System;
using UnityEngine.UI;

public class MySQLScript : MonoBehaviour
{
  
    private string sqlDBip = "linux.mme.dongguk.edu";
    private string sqlDBname = "s2015112614";
    private string sqlDBid = "s2015112614";
    private string sqlDBpw = null;
    private string sqlDBport = "33060";

    public Text Result;
    public Text[] SHOWTABLE;
    string sqlDatabase;
    string CommandText;
    string[] vCustomer = { "CustID", "Name", "Phone" };
    bool[] vCustomerINT = { true, false, false };

    string[] vProduct = { "ProdID", "Name", "Price"};
    bool[] vProductINT = { true, false, false };

    string[] vPart = { "PartID", "ProdID", "Name" };
    bool[] vPartFR = { false, true, false };
    bool[] vPartINT = { true, true, false };
    string[] vPartFRT = { "", "vProduct", "" };
    string[] vPartFRTN = { "", "Name", "" };

    string[] vOrder = { "OrderID", "CustID", "ProdID"};
    bool[] vOrderFR = { false, true, true};
    bool[] vOrderINT = { true, true, true};
    string[] vOrderFRT = { "", "vCustomer", "vProduct"};
    string[] vOrderFRTN = { "", "Name", "Name"};

    string[] vRepairman = { "RemID", "ProdID", "Name" };
    bool[] vRepairmanFR = { false, true, false };
    bool[] vRepairmanINT = { true, true, false };
    string[] vRepairmanFRT = { "", "vProduct", "" };
    string[] vRepairmanFRTN = { "", "Name", "" };

    string[] vRequest = { "RequID", "OrderID", "RemID" };
    bool[] vRequestFR = { false, true, true};
    bool[] vRequestINT = { true, true, true };
    string[] vRequestFRT = { "", "vOrder", "vRepairman"};
    string[] vRequestFRTN = { "", "OrderID", "Name"};

    string[] vRequestComplete = { "RequComID", "RequID", "PartID"};
    bool[] vRequestCompleteFR = { false, true, true };
    bool[] vRequestCompleteINT = { false, true, true };
    string[] vRequestCompleteFRT = { "", "vRequest", "vPart" };
    string[] vRequestCompleteFRTN = { "", "RequID", "Name" };
    MySqlConnection sqlconn;

    MySqlCommand Sender;
    MySqlDataReader Reader;
    MySqlDataReader Reader2;

    string[] text = new string[10];


    public void Start()
    {
        //DB정보 입력
        sqlDatabase = "Server=" + sqlDBip + ";port=" + sqlDBport + ";Database=" + sqlDBname + ";UserId=" + sqlDBid + ";Password=" + sqlDBpw + ";" + "CharSet=utf8";


        CommandText = "SELECT * FROM jobs;";
        sqlconn = new MySqlConnection(sqlDatabase);
        //접속 확인하기
        try
        { //접속
            sqlconn.Open();
          
            Sender = new MySqlCommand(CommandText, sqlconn);                 
            Debug.Log(sqlconn.State); //접속이 되면 OPEN이라고 나타남

            Debug.Log(Sender);

            //Return 값이 있을 때 사용한다.
            Reader = Sender.ExecuteReader();
            //Return 값이 없을 떄 사용한다.
            //Sender.ExecuteNonQuery();

            //접속 해제       
            Debug.Log(Reader);
            //sqlcmdall("DELETE FROM mongi");
        }
        catch (Exception msg)
        {
            Debug.Log(msg); //기타다른오류가 나타나면 오류에 대한 내용이 나타남
        }
        finally
        {
            sqlconn.Close();
        }
    }

    public void UpdateText(GameObject Head)
    {
        if (Head.transform.parent.gameObject.name == "INSERT")
        {
            bool CanInsert = true;
            int Count = 0;
            text.Initialize();
            text = new string[10];
            while (Head.transform.Find((Count + 1).ToString()))
            {
                Count++;
                text[Count] = Head.transform.Find((Count).ToString()).transform.Find("Text").GetComponent<Text>().text;
                if(text[Count] == "")
                {
                    Result.text = "값이 없습니다";
                    CanInsert = false;
                }
            }
            if (CanInsert)
            {
                if (Head.name == "vCustomer")
                {
                    // 단순 삽입
                    SQLInsert(Head.name, Count, vCustomer,vCustomerINT);
                }
                if (Head.name == "vProduct")
                {
                    // 단순 삽입
                    SQLInsert(Head.name, Count, vProduct,vProductINT);
                }
                if (Head.name == "vPart")
                {
                    // vProduct 참조
                    if (CheckINSERT(Head.name, Count, vPart, vPartFR, vPartFRT, vPartFRTN))
                    {
                        SQLInsert(Head.name, Count, vPart,vPartINT);
                    }
                    else
                    {
                        Result.text = "없는 물품입니다.";
                    }
                }
                if (Head.name == "vOrder")
                {
                    // vCustomer, vProduct 참조
                    if (CheckINSERT(Head.name, Count, vOrder, vOrderFR, vOrderFRT, vOrderFRTN))
                    {
                        SQLInsert(Head.name, Count, vOrder, vOrderINT);
                    }
                    else
                    {
                        Result.text = "없는 물품에 부품을 추가했습니다.";
                    }
                }
                if (Head.name == "vRepairman")
                {
                    if (CheckINSERT(Head.name, Count, vRepairman, vRepairmanFR, vRepairmanFRT, vRepairmanFRTN))
                    {
                        SQLInsert(Head.name, Count, vRepairman, vRepairmanINT);
                    }
                    else
                    {
                        Result.text = "없는 물품에 부품을 추가했습니다.";
                    }
                }
                if (Head.name == "vRequest")
                {
                    if (CheckINSERT(Head.name, Count, vRequest, vRequestFR, vRequestFRT, vRequestFRTN))
                    {
                        SQLInsert(Head.name, Count, vRequest, vRequestINT);
                    }
                    else
                    {
                        Result.text = "없는 물품에 부품을 추가했습니다.";
                    }
                }
                if (Head.name == "vRequestComplete")
                {
                    if (CheckINSERT(Head.name, Count, vRequestComplete, vRequestCompleteFR, vRequestCompleteFRT, vRequestCompleteFRTN))
                    {
                        SQLInsert(Head.name, Count, vRequestComplete, vRequestCompleteINT);
                    }
                    else
                    {
                        Result.text = "없는 물품에 부품을 추가했습니다.";
                    }
                }
            }
        }
        else if(Head.transform.parent.gameObject.name == "SELECT")
        {
            if (Head.name == "vCustomer")
            {
                SQLSelect(Head.name, vCustomer);
            }
            if (Head.name == "vProduct")
            {
                // 단순 삽입
                SQLSelect(Head.name, vProduct);
            }
            if (Head.name == "vPart")
            {
                SQLSelect(Head.name, vPart);
            }
            if (Head.name == "vOrder")
            {
                SQLSelect(Head.name, vOrder);
            }
            if (Head.name == "vRepairman")
            {
                SQLSelect(Head.name, vRepairman);
            }
            if (Head.name == "vRequest")
            {
                SQLSelect(Head.name, vRequest);
            }
            if (Head.name == "vRequestComplete")
            {
                SQLSelect(Head.name, vRequestComplete);
            }
        }
        else if (Head.transform.parent.gameObject.name == "DELETE")
        {
            text[1] = Head.transform.Find("1").transform.Find("Text").GetComponent<Text>().text;
            if (Head.name == "vCustomer")
            {
                SQLDelete(Head.name, text[1], vCustomer[1],vCustomerINT[1]);
            }
            if (Head.name == "vProduct")
            {
                // 단순 삽입
                SQLDelete(Head.name, text[1], vProduct[1], vProductINT[1]);
            }
            if (Head.name == "vPart")
            {
                SQLDelete(Head.name, text[1], vPart[1], vPartINT[1]);
            }
            if (Head.name == "vOrder")
            {
                SQLDelete(Head.name, text[1], vOrder[1], vOrderINT[1]);
            }
            if (Head.name == "vRepairman")
            {
                SQLDelete(Head.name, text[1], vRepairman[1], vRepairmanINT[1]);
            }
            if (Head.name == "vRequest")
            {
                SQLDelete(Head.name, text[1], vRequest[1], vRequestINT[1]);
            }
            if (Head.name == "vRequestComplete")
            {
                SQLDelete(Head.name, text[1], vRequestComplete[1], vRequestCompleteINT[1]);
            }
        }
    }

    bool CheckINSERT(string TABLE, int Count, string[] VALUES, bool[] ChecksFR, string[] FRTABLE, string[] FRTAName)
    {
        bool CanINSERT = false;
        for(int i = 0; i < VALUES.Length; i++)
        {
            if(ChecksFR[i] == true)
            {
                try
                {
                    OpenSQL();
                    CommandText = "SELECT * FROM " + FRTABLE[i] + " WHERE " + FRTAName[i] + " = '" + text[i] + "';";
                    Sender = new MySqlCommand(CommandText, sqlconn);
                    Reader = Sender.ExecuteReader();
                    while (Reader.Read())
                    {
                        Debug.Log("FIND");
                        CanINSERT = true;
                        text[i] = Reader[VALUES[i]].ToString();
                        Debug.Log(text[i]);
                    }
                    Result.text = CommandText;
                    Debug.Log(CommandText);
                    CloseSQL();
                }
                catch
                {
                    CloseSQL();
                }
            }
        }
        return CanINSERT;
    }


    void SQLInsert(string TABLE, int Count, string[] VALUES, bool[] INTCHECK)
    {
        try
        {
            int CountID = 0;
            bool Check = true;
            bool NOTOVERRAP = true;

            OpenSQL();
            CommandText = "SELECT * FROM " + TABLE + ";";
            Debug.Log(CommandText);
            Sender = new MySqlCommand(CommandText, sqlconn);
            Reader = Sender.ExecuteReader();
            while (Reader.Read())
            {
                int OverRapCount = 0;
                for (int i = 1; i < VALUES.Length; i++)
                {
                    Debug.Log(VALUES[i]);
                    Debug.Log(text[i]);
                    if(Reader[VALUES[i]].ToString() == text[i])
                    {
                        OverRapCount++;
                    }
                }
                if(OverRapCount == VALUES.Length - 1)
                {
                    NOTOVERRAP = false;
                    Check = false;
                    Result.text = "완전히 일치하는 값이 있습니다.";
                }
            }
            CloseSQL();
            
            while (Check)
            {
                OpenSQL();
                CommandText = "SELECT * FROM " + TABLE + " WHERE " +  VALUES[0] + " = " + CountID + ";";
                Debug.Log(CommandText);
                Sender = new MySqlCommand(CommandText, sqlconn);
                Reader = Sender.ExecuteReader();
                if(Reader.Read())
                {
                    Debug.Log("FIND");
                    CountID++;
                }
                else
                {
                    Check = false;
                }
                CloseSQL();
            }
            if (NOTOVERRAP)
            {
                OpenSQL();
                Debug.Log(CountID);
                MAKEValue(TABLE, VALUES);
                CommandText += "VALUES (";
                CommandText += CountID + ",";
                for (int i = 1; i <= Count; i++)
                {
                    if (!INTCHECK[i])
                        CommandText += "'";
                    CommandText += text[i];
                    if (!INTCHECK[i])
                        CommandText += "'";

                    if (i != Count)
                        CommandText += ",";
                }
                CommandText += ");";
                Result.text = CommandText;
                Debug.Log(CommandText);
                Sender = new MySqlCommand(CommandText, sqlconn);
                Sender.ExecuteNonQuery();
                Result.text += "\n\nSUCCESS";
                CloseSQL();
            }
        }
        catch (SystemException err)
        {
            Debug.Log(err);
            Result.text = "잘못 추가했습니다.";
            CloseSQL();
        }
    }

    void SQLSelect(string TABLE, string[] values)
    {
        try
        {
            OpenSQL();
            CommandText = "SELECT * FROM " + TABLE + ";";
            Sender = new MySqlCommand(CommandText, sqlconn);
            Reader = Sender.ExecuteReader();
            for(int i = 0; i < 4; i++)
            {
                SHOWTABLE[i].text = "";
            }
            for (int i = 0; i < values.Length; i++)
            {
                SHOWTABLE[i].text += values[i] + "\n";
            }
            while (Reader.Read())
            {
                //TAB만큼 띄어씌는 것이 안됨
                for (int i = 0; i < values.Length; i++)
                {
                    SHOWTABLE[i].text += Reader[values[i]].ToString() + "\n";
                }
            }
            CloseSQL();
        }
        catch
        {
            Result.text = "오류 발생";
            CloseSQL();
        }
    }

    void SQLDelete(string TABLE, string Keynumber, string ID, bool INTCHECK)
    {
        try
        {
            OpenSQL();
            CommandText = "DELETE FROM " + TABLE + " WHERE " + ID + " = ";
            if (!INTCHECK)
                CommandText += "'";

            CommandText += Keynumber;

            if (!INTCHECK)
                CommandText += "'";

            CommandText += ";";
            Sender = new MySqlCommand(CommandText, sqlconn);
            Result.text = CommandText;
            Sender.ExecuteNonQuery();
            CloseSQL();

            Result.text += "\n\nSUCCESS";
        }
        catch
        {
            Result.text = "오류 발생";
            CloseSQL();
        }
    }

    void MAKEValue(string TABLE, string[] VALUES)
    {
        CommandText = "INSERT INTO " + TABLE;
        CommandText += "(";
        for (int i = 0; i < VALUES.Length; i++)
        {
            CommandText += VALUES[i];
            if(i != VALUES.Length - 1)
                CommandText += ",";
        }
            CommandText += ")";
    }

    void OpenSQL()
    {
        sqlconn.Open();
    }

    void CloseSQL()
    {
        sqlconn.Close();
    }
}
