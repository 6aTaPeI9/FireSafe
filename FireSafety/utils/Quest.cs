using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FireSafety
{
    //Класс содержащий всю информацию о вопросах находящихся 
    //под одним индексом в базе
    [Serializable]
    public class Quest
    {
        public Quest(string data, string question, int right_answer)
        {
            Data = data;
            Question = question;
            RightAnswer = right_answer;
            SelectedIndex = -1;
        }

        public string Data { get; set; }
        public string Question { get; set; }
        public int RightAnswer { get; set; }
        public int Index { get; set; }
        public Quest Previous { get; set; }
        public Quest Next { get; set; }
        public int SelectedIndex { get; set; }
    }

    //Класс для работы с классом Quest
    [Serializable]
    public class LinkedQuestList
    {
        Quest last; //Последний элемент
        Quest first; //Первый элемент
        Quest curent; //Текущий элемент который видит пользователь
        public int count = 0; //Количество вопросов

        //Добавляем элемент в класс Quest
        public void Add(string data, string question, int right_answer)
        {
            Quest node = new Quest(data, question, right_answer);

            if (curent == null)
            {
                node.Next = null;
                node.Previous = null;
                node.Index = count;
                curent = node;
                last = node;
                first = node;

            }
            else
            {
                last.Next = node;
                node.Previous = last;
                node.Index = count;
                last = node;
            }
            count++;
        }

        //Получаем данные предидущего элемента и переключаем текущий
        public Quest GetPrev()
        {
            if (curent.Previous != null)
            {
                curent = curent.Previous;
                return curent;
            }
            else
            {
                return null;
            }
        }

        //Получаем данные следующего элемента и переключаем текущий
        public Quest GetNext()
        {
            if (curent.Next != null)
            {
                curent = curent.Next;
                return curent;
            }
            else
            {
                return null;
            }
        }

        //Получаем данные текущего элемента
        public Quest GetCur()
        {
            if (curent != null)
                return curent;
            else
                return null;
        }

        //Устанавливает элемент текущим по индексу.
        public void SetCurByIndex(int index)
        {
            if (index > count)
                return;

            Quest iter = first;
            for (int i = 1; i < this.count; i++)
            {
                if (iter.Index == index)
                {
                    curent = iter;
                }
                else
                    iter = iter.Next;
            }
        }

        public int GetHappyIndex()
        {
            int right_count = 0;
            Quest iter;
            iter = first;
            for (int i = 0; i <= this.count - 1; i++)
            {

                if (iter.SelectedIndex == -1)
                    return -1;

                if (iter.SelectedIndex == iter.RightAnswer)
                    right_count++;

                iter = iter.Next;
            }
            return right_count;
        }

        public bool IsLast()
        {
            if (curent.Next is null)
                return true;
            else
                return false;
        }

        public int GetCurIndex()
        {
            return curent.Index;
        }

        public void SetSelectedItem(int index)
        {
            curent.SelectedIndex = index;
        }

        public int GetSelectedItem()
        {
            return curent.SelectedIndex;
        }
    }
}
