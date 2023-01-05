using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://wonsj09:wsj@cluster0.77rho6r.mongodb.net/?retryWrites=true&w=majority");
    // Start is called before the first frame update
    public IMongoDatabase database = null;
    public IMongoCollection<BsonDocument> collection;
    void Start()
    {
        database = client.GetDatabase("user");
        collection = database.GetCollection<BsonDocument>("userlist");

        /*collection.InsertOne(new BsonDocument(new Dictionary<string, object>
        {
            ["Name"] = "Name of account",
            ["Email"] = "myemail@account.com",
            ["Password"] = "don't ever do that to me. I hope this is a hash...",
            ["SomeOtherField"] = "don't need it"
        }));
        var projection = Builders<BsonDocument>.Projection.Include("Name").Include("Email").Include("Password").Exclude("_id");*/

        /*var fillter = Builders<BsonDocument>.Filter.Eq("Name", "�Ƶ�");//ã�� ��ť��Ʈ�� Name�� �Ƶ��ΰ�
        var nullFillter = collection.Find(fillter).FirstOrDefault();//if null �̸� ã�� ����
        if (nullFillter != null)
        {
           Debug.Log(nullFillter.GetValue("Name"));
        }*/
            //foreach (BsonDocument doc in documents)
            //{
            //    Debug.Log($"Name: {doc.GetValue("Name")}\n" +
            //              $"Email: {doc.GetValue("Email")}\n" +
            //              $"Password: {doc.GetValue("Password")}");
            //}
        //var address = nested["address"].AsBsonDocument;
        //Console.WriteLine(address["city"].AsString);
        //// or, jump straight to the value ...
        //Console.WriteLine(nested["address"]["city"].AsString);
        //// loop through the fields array
        //var allFields = nested["fields"].AsBsonArray;
        //foreach (var fields in allFields)
        //{
        //    // grab a few of the fields:
        //    Console.WriteLine("Name: {0}, Type: {1}",
        //        fields["NAME"].AsString, fields["TYPE"].AsString);
        //}
        // var document = new BsonDocument { {"id:","�Ƶ�"}, {"password:", "���"}, {"nickname:", "�г���"}, { "nickname:", "�г���" } };

        //var nullFillter = collection.Find(fillter).FirstOrDefault();//if null �̸� ã�� ����

        //var update = Builders<BsonDocument>.Update.Set("merge:", "����");//ã���� �ٲٱ�
        //collection.UpdateOne(fillter, update);//�ٲٴ��Լ�
        //collection.Find(document);
        //GetScoresFromDataBase();
    }

    /*public async void SaveScoreToDataBase(string userName, int score)
    {
        var document = new BsonDocument { { userName, score } };
        await collection.InsertOneAsync(document);
    }*/

    public void Login()
    {
        var fillter = Builders<BsonDocument>.Filter.Eq("address", GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address);//ã�� ��ť��Ʈ�� Name�� �Ƶ��ΰ�
        var nullFillter = collection.Find(fillter).FirstOrDefault();//if null �̸� ã�� ����
        if (nullFillter == null)
        {
            Debug.Log("ȸ������");
            collection.InsertOne(new BsonDocument(new Dictionary<string, object>
            {
                ["address"] = GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address,
                ["username"] = GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.username,
            }));
            var update = Builders<BsonDocument>.Update.Set("inventory", inventoryData) ;//ã���� �ٲٱ�
            collection.UpdateOne(fillter,update);
        }
        else
        {
            BsonValue value = null;
            BsonValue value2 = null;
            string number=null;
            for (int i = 0; i < 10; i++)
            {
                nullFillter.TryGetValue("inventory", out value);
                if (i == 0) number = "one";
                else if (i == 1) number = "two";
                else if (i == 2) number = "three";
                else if (i == 3) number = "four";
                else if (i == 4) number = "five";
                else if (i == 5) number = "six";
                else if (i == 6) number = "seven";
                else if (i == 7) number = "eight";
                else if (i == 8) number = "nine";
                else if (i == 9) number = "ten";
                value.ToBsonDocument().TryGetValue(number, out value);
                if (value.ToString() == "BsonNull") continue;// �ش� ĭ�� ���� ���ٸ� �������� �̵�
                Debug.Log(number);
                    CustomDeck customDeck = new CustomDeck();
                for (int num = 1; num < 7; num++)
                {
                    Debug.Log(value);
                    value.ToBsonDocument().TryGetValue($"tier_{num}", out value2);
                    string[] valueSplit = value2.ToString().Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                    for (int j = 0; j < valueSplit.Length; j++)
                    {
                        if (num == 1) customDeck.tier_1.Add(valueSplit[j]);
                        else if (num == 2) customDeck.tier_2.Add(valueSplit[j]);
                        else if (num == 3) customDeck.tier_3.Add(valueSplit[j]);
                        else if (num == 4) customDeck.tier_4.Add(valueSplit[j]);
                        else if (num == 5) customDeck.tier_5.Add(valueSplit[j]);
                        else if (num == 6) customDeck.tier_6.Add(valueSplit[j]);
                        Debug.Log(valueSplit[j]);
                    }
                }
                inventoryData.AddCustomDeck(customDeck);
                Debug.Log("?");
            }
            GameMGR.Instance.uIMGR.SetParentPackAddButton();
        }

    }
    public InventoryData inventoryData = new InventoryData();
    public void InsertInventoryData()
    {
        var fillter = Builders<BsonDocument>.Filter.Eq("address", GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address);//ã�� ��ť��Ʈ�� Name�� �Ƶ��ΰ�
        var update = Builders<BsonDocument>.Update.Set("inventory", inventoryData);//ã���� �ٲٱ�
        collection.UpdateOne(fillter, update);
    }
}
public class InventoryData
{
    public CustomDeck one;
    public CustomDeck two;
    public CustomDeck three;
    public CustomDeck four;
    public CustomDeck five;
    public CustomDeck six;
    public CustomDeck seven;
    public CustomDeck eight;
    public CustomDeck nine;
    public CustomDeck ten;
    public void AddCustomDeck(CustomDeck customDeck)
    {
        if (one == null)
        {
            one = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(one);
        }
        else if (two == null)
        {
            two = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(two);
        }
        else if (three == null)
        {
            three = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(three);
        }
        else if (four == null)
        {
            four = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(four);
        }
        else if (five == null)
        {
            five = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(five);
        }
        else if (six == null)
        {
            six = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(six);
        }
        else if (seven == null)
        {
            seven = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(seven);
        }
        else if (eight == null)
        {
            eight = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(eight);
        }
        else if (nine == null)
        {
            nine = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(nine);
        }
        else if (ten == null)
        {
            ten = customDeck;
            GameMGR.Instance.uIMGR.CreateMyPackButton(ten);
        }
        }
    }
public class CustomDeck
{
    public List<string> tier_1 = new List<string>();
    public List<string> tier_2 = new List<string>();
    public List<string> tier_3 = new List<string>();
    public List<string> tier_4 = new List<string>();
    public List<string> tier_5 = new List<string>();
    public List<string> tier_6 = new List<string>();
}
