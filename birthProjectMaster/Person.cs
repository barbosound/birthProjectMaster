using System;
using System.Drawing;

namespace birthProjectMaster
{
    public class Person
    {
        private string name;
        private DateTime birth;
        private Image img;
        private string imgPath;

        public Person(string name, DateTime birth, string path)
        {
            this.name = name;
            this.birth = birth;
            this.imgPath = path;
        }

        public string getName()
        {
            return this.name;
        }
        public DateTime getBirth()
        {
            return this.birth;
        }
        public Image getImg()
        {
            return this.img;
        }
        public void setImg(Image img)
        {
            this.img = img;
        }

        public string getImgPath()
        {
            return this.imgPath;
        }
        public void setImgPath(string imgPath)
        {
            this.imgPath = imgPath;
        }
    }

}
