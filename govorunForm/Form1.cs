using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace govorunForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = textBox1;
            textBox1.Focus();
            rnd = new Random();
            MessageBox.Show("Задавая вопросы, не забудьте вопросительный знак."+"\n"+
                "Вводите текст с заглавной буквы." + "\n" +
                "Чтобы задать тему, упомяните ее название." + "\n" +
                "Скажите \"Хватит\", чтобы выключить тему." + "\n" +
                "Попрощайтесь, например \"Пока\", и беседа закончится.", "Начало");
            hey = File.ReadAllLines(hello); // чтение приветствий
            keyname = File.ReadAllLines(keynam); //чтение ключей тем
            keyfiles = File.ReadAllLines(keyfolder); //чтение каталогов тем
            richTextBox1.AppendText("Говорун: " + hey[rnd.Next(hey.Length - 1)] + "\n");
        }
        private Random rnd;
        //имена неизменяемых файлов
        private string strangeAnswers = "strange answers.txt", answersToAnswers = "answers to answers.txt"; 
        private string gbye = "goodbye.txt", hello = "hello.txt", keynam = "key.txt", keyfolder= "key folder name.txt";
        //имена изменяемых файлов
        private string[] nameFiles = { "answers to questions.txt", "questions.txt", "user questions.txt"};
        private string[] keyNameFiles = { "answers to questions.txt", "questions.txt", "user questions.txt" };
        //переключение на запись ответов пользователя и просто вывод ответов
        bool dontRemember = true;
        bool replyToAnswer = false; 
        //ответы на ответы, расплывчатые ответы, шаблонные вопросы,приветствия,прощания, ключи тем, каталоги тем
        private string[] ansToAns, strangeAns, quest, hey, bye, keyname, keyfiles; 
        private int numbQue; //номер вопроса
        //метод чтения не модифицируемых файлов вопросов и ответов
        void readingFiles()
        {
            quest = File.ReadAllLines(keyNameFiles[1]); //чтение вопросов
            ansToAns = File.ReadAllLines(answersToAnswers); //чтение ответов на ответы
            strangeAns = File.ReadAllLines(strangeAnswers); //чтение расплывчатых ответов 
            bye = File.ReadAllLines(gbye); //чтение прощаний
            

        }

        //выдать шаблонный вопрос из "questions.txt"
        //и записать ответ пользователя в "answers to questions.txt"
        public void formulaicQueAns()
        {
            //вывод вопроса
            numbQue = rnd.Next(quest.Length - 1); //случайно выбираем вопрос
            richTextBox1.AppendText("Говорун: " + quest[numbQue] + "\n");
            //переключение на сохранение вопроса
            dontRemember = false;
        }
        //выводим не шаблонный вопрос
        public void strangeQueAns()
        {
            string[] userQuest = File.ReadAllLines(keyNameFiles[2]); //не шаблонные вопросы
            int nomerVoprosa = rnd.Next(userQuest.Length - 1);
            richTextBox1.AppendText("Говорун: " + userQuest[nomerVoprosa]+"\n");
            replyToAnswer = true; //ответить на ответ пользователя
        }

        //анализ текста, введенного пользователем
        public void analysText(string userText)
        {
            bool isquestion = false; //это вопрос
            bool ishello = false; //это приветствие
            bool isbye = false; //это прощяние

            //проверка на вопрос
            if (userText.Contains('?')) //это вопрос
            {
                for (int i = 0; i < quest.Length; i++)//сверяем с файлом шаблоных вопросов "questions.txt"
                {
                    if (userText.Contains(quest[i]))// i-й вопрос найден
                    {//вывести соответствующий ответ
                        string[] ansToQue = File.ReadAllLines(keyNameFiles[0])[i].Split(", "); //чтение ответов в i-й строке "answers to questions.txt"
                        richTextBox1.AppendText("Говорун: " + ansToQue[rnd.Next(ansToQue.Length - 1)] + "\n"); //вывод одного ответа
                        isquestion = true; //это шаблонный вопрос
                        formulaicQueAns(); //задать шаблонный вопрос
                        return;
                    }
                }
                if (!isquestion) //это вопрос, но не шаблонный
                {//дать расплывчатый ответ
                    File.AppendAllText(keyNameFiles[2], "\r\n" + userText);//записать вопрос пользователя в файл "user questions.txt"
                    richTextBox1.AppendText("Говорун: " + strangeAns[rnd.Next(strangeAns.Length - 1)] + "\n");
                    formulaicQueAns(); //задать шаблонный вопрос
                    return;
                }
            }
            else //это не вопрос
            {
                if (!isquestion)
                {
                    for (int i = 0; i < hey.Length; i++)
                    {
                        if (userText.Contains(hey[i]))//это приветствие
                        {
                            ishello = true; 
                            formulaicQueAns(); //задать вопрос
                            return;
                        }
                    }
                    if (!ishello)//это не вопрос и не приветствие
                    {
                        for (int i = 0; i < bye.Length; i++)
                        {
                            if (userText.Contains(bye[i]))//это прощание
                            {//вывести прощание
                                richTextBox1.AppendText("Говорун: " + bye[rnd.Next(bye.Length - 1)] + "\n");
                                isbye = true;
                                textBox1.Enabled = false;
                                DialogResult result = MessageBox.Show("Хорошо поболтали!", "Конец");
                                if (result == DialogResult.OK) this.Close(); //завершить программу
                            }
                        }
                        //это не вопрос, не приветствиене и не прощание
                        if (!isbye)
                        {//вывести ответ на ответ
                            richTextBox1.AppendText("Говорун: " + ansToAns[rnd.Next(ansToAns.Length - 1)] + "\n");
                            strangeQueAns(); //задать не шаблонный вопрос
                            return;
                        }
                    }
                }
            }
        }
        bool searchKey = true; //искать ключ

        //анализ текста на содержание слова "Хватит" или ключей тем
        void analysKey(string text)
        {
            int key = -1, i = 0;

            if (text.Contains("Хватит")) //сбросить ключ
            {
                for (i = 0; i < nameFiles.Length; i++) //убрать название каталога
                {
                    keyNameFiles[i] = nameFiles[i]; //дефолтные файлы
                }
                searchKey = true; //снова анализировать строку на наличие ключа
            }
            if(searchKey) //поиск ключа в строке
            {
                while (i < keyname.Length  && key < 0)
                {
                   
                   if (text.Contains(keyname[i])) key = i; //присвоить номер ключа
                    i++;
                }
                if (key >= 0) //ключ найден
                {
                    for (i = 0; i < nameFiles.Length; i++)
                    {// заменить названия файлов, добавив имя тематического каталога
                        keyNameFiles[i] = keyfiles[key] + nameFiles[i];
                    } 
                    searchKey = false; //не искать ключ до ввода "хватит"
                }  
            }
        }

        //обработчик нажатия пользователем Enter в поле ввода
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBox1.Text!="")
            {
                string userText = textBox1.Text; //получаем текст, введенный пользователем
                readingFiles(); //считываем неизменяемые вопросы и ответы
                e.SuppressKeyPress = true;
                richTextBox1.AppendText("Вы: " + userText + "\n"); //выводим текст, введенный пользователем
                analysKey(userText); //ищем ключ
                //только ответить на ответ
                if (replyToAnswer)
                {
                    richTextBox1.AppendText("Говорун: " + ansToAns[rnd.Next(ansToAns.Length - 1)] + "\n");//отвечаем на ответ
                    replyToAnswer = false;
                }
                else
                {//не запоминать ответ пользователя
                    if (dontRemember) analysText(textBox1.Text); //анализируем текст
                    else //запоминаем ответ на шаблонный вопрос
                    {
                        richTextBox1.AppendText("Говорун: " + ansToAns[rnd.Next(ansToAns.Length - 1)] + "\n");//отвечаем на ответ
                        //записываем ответ в файл
                        string[] ansToQue = File.ReadAllLines(keyNameFiles[0]); //счтитываем файл с ответами
                        if (!ansToQue[numbQue].Contains(userText)) //если такого ответа еще нет
                        {//записываем ответ в строку, соответствующую вопросу
                            ansToQue[numbQue] = ansToQue[numbQue] + ", " + userText;
                        }
                        File.WriteAllLines(keyNameFiles[0], ansToQue);
                        dontRemember = true;
                    }
                }
                textBox1.Clear();
            }

        }
    }
}


