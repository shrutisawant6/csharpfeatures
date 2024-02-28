using Bogus;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CSharpImprovements
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Some new C# features to use.");
            Console.WriteLine("****************************");

            /* 
             finding first record(Justin) from 10,00,000 record
             find name using Count: 26.4798
             find name using Any: 0.2639

             finding intermediate record(Chris) from 10,00,000 record
             find name using Count: 20.6042
             find name using Any: 0.2799

             finding last record(Random last name) from 10,00,000 record
             find name using Count: 18.0864
             find name using Any: 15.3058
            */
            //AnyOverCount1();
            //AnyOverCount2();


            ////Extended property pattern
            //ExtendedPropPatterns();

            ////ThrowIfNull<T>
            //ThrowIfNullException();

            //faster for loop, results not consistent and performance of both appear to be same
            //FasterLoopJI();
            //FasterLoopIJ();

            ////LINQ All vs TrueForAll on 1000 records
            /*
             TrueForAll LINQ: 0.1825 //faster
             All LINQ: 0.3157
             */
            //TrueForAllLINQ();
            //AllLINQ();


            //SQL injection scenario
            /*
             -- avoid LIKE
             -- use proper parameters 
             -- SELECT TOP 1 when needed
             -- SELECT *, rather SELECT ID, BATCHID, NAME
             */
            //GetProductBasedOnBatchId("55889635");
            //GetProductBasedOnBatchId("1 OR 1=1");//sql injection attack(all table details received
            //GetProductBasedOnBatchId_NoSQLInjectionAttack("55889635");
            //GetProductBasedOnBatchId_NoSQLInjectionAttack("1 OR 1=1");//no sql injection attack


            ////pdf generator using QuestPDF
            //PDFGenerator();

            ////access private methods/fields
            //CallPrivateFieldsMethods();

            ////improve regex
            //ImproviseRegex();

            ////Right way to throw exception
            //RethrowException();

            //Class VS Record VS Struct
            //ClassRecordStruct();
        }

        //https://www.youtube.com/watch?v=eqqBzwIIM-4
        public static void AnyOverCount1()
        {
            var faker = new Faker<Person>()
                .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                .UseSeed(1);

            var names = faker.Generate(1000000);
            names[999999].FirstName = "Random last name";

            Stopwatch sw1 = Stopwatch.StartNew();
            var find1stUsingCount = names.Count(n => n.FirstName == "Chris") > 0;//Random last name//Elena//Justin//Chris
            TimeSpan elapsedCount = sw1.Elapsed;
            Console.WriteLine($"find name using Count: {elapsedCount.TotalMilliseconds}ms");
        }

        public static void AnyOverCount2()
        {
            var faker = new Faker<Person>()
                .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                .UseSeed(1);

            var names = faker.Generate(1000000);
            names[999999].FirstName = "Random last name";

            Stopwatch sw = Stopwatch.StartNew();
            var find1stUsingAny = names.Any(n => n.FirstName == "Chris");//Random last name//Elena//Justin//Chris
            TimeSpan elapsedAny = sw.Elapsed;
            Console.WriteLine($"find name using Any: {elapsedAny.TotalMilliseconds}ms");

        }

        //applicable on record types only
        public static void WithExpression()
        {
            var feature1 = new CSharpFeature { Name = "global using directive" };
            //var feature2 = feature1 with { Name = "4" }; //applicable on record types
        }

        //https://www.youtube.com/watch?v=wqDYj0be_og
        public static void ExtendedPropPatterns()
        {
            const string valueToBeCompared = "Worsens health";
            var food = new Food
            {
                Name = "Carbonated drinks",
                //Usage = new FoodUsage
                //{
                //    Name = "Worsens health",
                //    Description = new FoodIngredient
                //    {
                //        Ingredients = new List<string>
                //        {
                //            "water", "carbon dioxide", "sweeteners", "flavoring", "colors", "acids"
                //        }
                //    }
                //}
            };

            //if (food.Usage.Name == valueToBeCompared)
            //    Console.WriteLine("WITHOUT Extended property pattern.");

            if (food?.Usage?.Name == valueToBeCompared)
                Console.WriteLine("WITHOUT Extended property pattern.");

            if (food is Food { Usage: { Name: valueToBeCompared } })//extended null checks as well
                Console.WriteLine("WITHOUT Extended property pattern.");

            if (food is Food { Usage.Name: valueToBeCompared })//preferred : extended null checks as well
                Console.WriteLine("WITH Extended property pattern.");

            //This improvement is not a revolution, but a very welcome evolution.
        }

        public static void ExtendedPropPatterns1()
        {
            const string valueToBeCompared = "to simplify working with namespaces";

            var feature = new CSharpFeature
            {
                Name = "global using directive",
                Usage = new CSharpFeatureUsage
                {
                    Use = valueToBeCompared,
                    AdditionalInfo = "If the keyword global appears prior to a using directive, that using applies to the entire project."
                }
            };

            if (feature is CSharpFeature { Usage: { Use: valueToBeCompared } })
                Console.WriteLine("Thanks to 'global using directive'.");

            if (feature is CSharpFeature { Usage.Use: valueToBeCompared }) // Extended property pattern
                Console.WriteLine("Thanks to 'global using directive' > using Extended property pattern.");
        }

        public static void ThrowIfNullException(string? value = null)
        {
            if (value == RandomNumberGenerator.Create().ToString())
                value = "Not empty any more.";

            //null value is checked outside and later exception is thrown
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            //ArgumentNullException.ThrowIfNull > null value is checked within exception
            ArgumentNullException.ThrowIfNull(value);
        }

        public void IsAndIsNotOperator()
        {
            var value = "Use 'is/is not' operator for good readability";
            if (value is null)
                Console.WriteLine("We have use 'is' operator.");

            if (value is not null)
                Console.WriteLine("We have use 'is not' operator.");
        }

        //https://stackoverflow.com/questions/15062518/iterating-through-matrix-is-slower-when-changing-aij-to-aji
        //https://code-maze.com/csharp-tips-improve-quality-performance/
        //row wise 
        public static void FasterLoopIJ()
        {
            var list = new List<List<int>>{
                                            new() { 1, 2, 3, 10, 11 },
                                            new() { 4, 5, 6, 12, 13 },
                                            new() { 7, 8, 9, 14, 15 },
                                            new() { 4, 5, 6, 12, 13 },
                                            new() { 1, 2, 3, 10, 11 }
                                          };

            Stopwatch sw1 = Stopwatch.StartNew();
            var finalCount = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (list[i][j] > 0)
                    {
                        finalCount++;
                    }
                }
            }
            Console.WriteLine(finalCount);
            TimeSpan elapsed1 = sw1.Elapsed;
            Console.WriteLine($"Loop performance: {elapsed1.TotalMilliseconds}ms");
        }

        //Column wise
        public static void FasterLoopJI()
        {
            var list1 = new List<List<int>>{
                                            new() { 1, 2, 3, 10, 11 },
                                            new() { 4, 5, 6, 12, 13 },
                                            new() { 7, 8, 9, 14, 15 },
                                            new() { 4, 5, 6, 12, 13 },
                                            new() { 1, 2, 3, 10, 11 }
                                          };

            Stopwatch sw2 = Stopwatch.StartNew();
            var finalCount = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (list1[j][i] > 0)
                    {
                        finalCount++;
                    }
                }
            }
            Console.WriteLine(finalCount);
            TimeSpan elapsed2 = sw2.Elapsed;
            Console.WriteLine($"Loop performance: {elapsed2.TotalMilliseconds}ms");
        }

        //try out sql injection //https://www.youtube.com/watch?v=fRhsU4K82Fg
        private static void GetProductBasedOnBatchId(string batchId)
        {
            string connectionString = "User ID=TestUser123;Password=@789TestUser$;Initial Catalog=MyDatabase;Data Source=SYSLP429\\SQLEXPRESS;Trusted_Connection=true;TrustServerCertificate=True";

            string queryString = $"SELECT * FROM FOOD WHERE BatchId = {batchId}";

            using SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryString, connection);
            command.Connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Id\tBatchId\t\tName\t");
            while (reader.Read())
            {
                Console.WriteLine(string.Format("{0}\t|{1}\t|{2}", reader[0], reader[1], reader[2]));
            }
        }

        private static void GetProductBasedOnBatchId_NoSQLInjectionAttack(string batchId)
        {
            string connectionString = "User ID=TestUser123;Password=@789TestUser$;Initial Catalog=MyDatabase;Data Source=SYSLP429\\SQLEXPRESS;Trusted_Connection=true;TrustServerCertificate=True";

            string queryString = "SELECT TOP 1 Id, BatchId, Name FROM FOOD WHERE BatchId = @batchId";

            using SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryString, connection);
            command.Parameters.Add(new SqlParameter("batchId", batchId));
            command.Connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Id\tBatchId\t\tName\t");
            while (reader.Read())
            {
                Console.WriteLine(string.Format("{0}\t|{1}\t|{2}", reader[0], reader[1], reader[2]));
            }
        }


        //if else, rather use switch case string to string //https://www.youtube.com/watch?v=kZwIaEAdF0I

        //loop over a list //https://www.youtube.com/watch?v=jUZ3VKFyB-A

        //LINQ //https://www.youtube.com/watch?v=cpL-fuiEfwU
        public static void AllLINQ()
        {
            var random = new Random(500);
            var list = Enumerable.Range(0, 1000).Select(r => random.Next(1, int.MaxValue)).ToList();

            Stopwatch sw = Stopwatch.StartNew();
            list.All(n => n > 0);
            TimeSpan elapsed = sw.Elapsed;
            Console.WriteLine($"All LINQ: {elapsed.TotalMilliseconds}ms");
        }

        public static void TrueForAllLINQ()
        {
            var random = new Random(500);
            var list = Enumerable.Range(0, 1000).Select(r => random.Next(1, int.MaxValue)).ToList();

            Stopwatch sw = Stopwatch.StartNew();
            list.TrueForAll(n => n > 0);
            TimeSpan elapsed = sw.Elapsed;
            Console.WriteLine($"TrueForAll LINQ: {elapsed.TotalMilliseconds}ms");
        }

        //QuestPDF https://www.youtube.com/watch?v=_M0IgtGWnvE
        public static void PDFGenerator()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            string header = "How are you eating!";
            string context = "Health depends on their eating habits.Studies have shown that healthy meal choices can help people avoid developing health problems in the future and enhance children's cognitive abilities. As a result, it's critical to instill excellent eating habits in children as early as possible, such as the Best Boarding School in Udaipur with hostel facilities does.Consuming these nutrients in a structured manner during the early years can help lay the foundation for a healthy life.One of the most important aspects of healthy eating is portion control while reducing fat and sugar consumption in meals to acceptable levels. Let's look at the top ten eating habits that students should follow:" +
                    "\r\n\r\nChew your food well.\r\nEat a lot of fibre.\r\nDrink lots of water.\r\nKeep healthy snacks around.\r\nDon't skip breakfast.\r\nInclude calcium-rich foods in your diet.\r\nAvoid sugar.\r\nEat colorful food.\r\n\r\n\r\n1. Chew your food well.\r\nSlower eating is one of the essential healthy eating habits for children. Children who eat gradually enjoy a sense of fulfillment. Children must be taught about when they are hungry. Children need light meals since they are quickly digested, which is excellent for keeping their energy levels up.\r\n\r\n2. Eat a lot of fibre.\r\nFiber is considered to be vital for our bodies and weight equilibrium. Fiber is a vital component of children's diets in Best Boarding School for Girls, which aids digestion, maintains glucose levels, lowers cholesterol levels, and prevents heart disease. Fiber may be found in fruits, vegetables, whole grain products, and nuts. If children enjoy eating bread, they should eat it with whole wheat bread.\r\n\r\n3. Drink lots of water.\r\nWater is an essential aspect of a person's health. As a result, children must be educated to drink at least 2 liters of water each day, and more if required. This will assist them in learning effectively, staying healthy, avoiding headaches, and avoiding dehydration.\r\n\r\n4. Keep healthy snacks around.\r\nChildren frequently become hungry throughout the day. As a result, they should be taught how to eat nutritious snacks and avoid losing concentration. That is why students from the International Boarding School in India bring nutritious food to prevent turning to junk when they cannot obtain anything else.\r\n\r\n5. Don't skip breakfast.\r\nSkipping meals lowers academic performance. Children frequently skip their breakfast in a hurry, which is not advised. They must never leave without eating breakfast since it leaves them hungry and makes it more difficult for them to memorize and learn. A child who doesn't have enough time to eat a balanced meal before going to school may drink juice, fruit, and other similar items.\r\n\r\n6. Include calcium-rich foods in your diet.\r\nIt's critical to include calcium-rich meals in children's diets. Children who attend the Best Boarding School in Udaipur with a hostel facility get calcium from a young age, which helps their bodies avoid osteoporosis later in life. Children who don't like milk can try to add lots of low-fat yogurt, low-fat cheese, and green leafy vegetables to their diet.\r\n\r\n7. Avoid sugar.\r\nSugar is considered to be the most harmful for good nutrition. It may provide energy at times, but it delivers the wrong sort of energy. The sugar in them causes us to feel drowsy and sleepy. Soda, sweets, and other sugary foods should be avoided in a child's diet. Children must consume dried fruit, almonds, fresh fruit, dark chocolate, or similar meals that provide all the energy they require to replace sugar.\r\n\r\n8. Eat colorful food.\r\nColorful food is eating a lot of fruits and vegetables rather than junk food. According to one of the finest Boarding schools in Udaipur, children should eat a rainbow of colors. This will undoubtedly provide them with a wider range of nutrients rather than providing them with the same nutrients all the time. Children may also select from apples, sweet potatoes, peas, spinach, watermelon, blueberries, and so on as options. The more hues they include in their diet, the better for them.\r\n\r\n9. Plan for snacks.\r\nConstant munching might frequently result in overindulgence. As a result, snacks must be prepared at particular intervals during the day as part of a balanced diet, and it does not impact a youngster's appetite. Children should establish the habit of snacking on healthy food items like dry fruits, fox nuts, and seeds, among other things.\r\n\r\n10. Don't forget that at the International Boarding School in India, physical activity is essential.\r\nHealthy lifestyles, therefore, necessitate engaging inappropriate physical exercise. Children should participate in at least 1 to 2 hours of sports activities each day, including trips to the park, jogging, sports, and age-appropriate sports.\r\n\r\n";

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string image = Path.Combine(currentDirectory, @"..\..\..\PDFFiles\Health.jpg");
            string imagePath = Path.GetFullPath(image);

            string pdfFolder = Path.Combine(currentDirectory, @"..\..\..\PDFFiles");
            string pdfFolderPath = Path.GetFullPath(pdfFolder);
            string newPDFFile = $"{pdfFolderPath}\\HowToStayHealthy_{DateTime.UtcNow.Ticks}.pdf";

            Document.Create(contatiner =>
            {
                contatiner.Page(page =>
                {
                    page.PageColor(Colors.Lime.Lighten4);

                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.Header().AlignCenter()
                    .Text(header)
                    .FontSize(20).FontColor(Colors.Lime.Darken4);


                    page.Content()
                    .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                    .Column(txt =>
                    {
                        txt.Item().Image(imagePath);
                        //txt.Item().Background(Colors.LightGreen.Lighten5);
                        txt.Item().PaddingBottom(1, QuestPDF.Infrastructure.Unit.Centimetre);
                        txt.Item().Text(context).FontColor(Colors.BlueGrey.Darken1);
                    });



                    page.Footer().AlignCenter()
                    .Text(txt =>
                    {
                        txt.CurrentPageNumber();
                    });
                });
            }).GeneratePdf(newPDFFile);
            //.ShowInPreviewer();
        }

        //access private methods/fields https://www.youtube.com/watch?v=WqeSRUXJ9VM
        public static void CallPrivateFieldsMethods()
        {
            var limitedAccess = new LimitedAccessClass();

            var privateMethod = typeof(LimitedAccessClass)
                .GetMethod("PrivateMethod", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(limitedAccess, Array.Empty<object>());

            Console.WriteLine($"PrivateMethod return value: {privateMethod}");


            var privateField = typeof(LimitedAccessClass)
                .GetField("PrivateField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(limitedAccess);

            Console.WriteLine($"PrivateField value: {privateField}");
        }


        //improvise regex, 1stly try to avoid it https://www.youtube.com/watch?v=NOLn0QwGlEE
        private static void ImproviseRegex()
        {
            //email check regex

            var email = "aaaaaaaaaaaaaaaaaaaaaaaaaaaa>";
            //"shruti@s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.@s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.s.c ";

            var regexInitial = "(a+)+$";
            //@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            //simple regex match
            Stopwatch sw = Stopwatch.StartNew();
            bool isEmail = Regex.IsMatch(email, regexInitial);
            TimeSpan elapsedCount = sw.Elapsed;
            Console.WriteLine($"1st Regex: {isEmail} {elapsedCount.TotalMilliseconds}ms"); //1st Regex: False 55211.8081ms

            //with ExplicitCapture + Compiled
            Stopwatch sw1 = Stopwatch.StartNew();
            bool isEmail1 = Regex.IsMatch(email, regexInitial, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            TimeSpan elapsedCount1 = sw1.Elapsed;
            Console.WriteLine($"2nd Regex: {isEmail1} {elapsedCount1.TotalMilliseconds}ms"); //2nd Regex: False 34.9966ms

            //if no solution, use timeout
            try
            {
                Stopwatch sw2 = Stopwatch.StartNew();
                bool isEmail2 = Regex.IsMatch(email, regexInitial, RegexOptions.None, TimeSpan.FromMilliseconds(1));
                TimeSpan elapsedCount2 = sw2.Elapsed;
                Console.WriteLine($"3rd Regex: {isEmail2} {elapsedCount2.TotalMilliseconds}ms"); //Match timed out! The RegEx engine has timed out while trying to match a pattern to an input string. This can occur for many reasons, including very large inputs or excessive backtracking caused by nested quantifiers, back-references and other factors.

            }
            catch (RegexMatchTimeoutException ex)
            {
                Console.WriteLine($"Match timed out! {ex.Message} ");
            }
        }

        private static void RethrowException()
        {
            try
            {
                ThrowIfNullException();
            }
            catch (Exception ex)
            {
                /*
                 * Unhandled exception. System.ArgumentNullException: Value cannot be null. (Parameter 'value')
                   at CSharpEnhc.Program.ThrowIfNullException(String value) in ...
                   at CSharpEnhc.Program.RethrowException(Int32 param1, Int32 param2) in ...
                   at CSharpEnhc.Program.Main(String[] args) in E:\CodeTrial\CSharp\NewEnhancements\CSharpEnhc\Program.cs:line 95
                 */
                throw;

                /*
                 * Unhandled exception. System.ArgumentNullException: Value cannot be null. (Parameter 'value')																																 
                   at CSharpEnhc.Program.RethrowException(Int32 param1, Int32 param2) in ...
                   at CSharpEnhc.Program.Main(String[] args) in 
                 */
                //throw ex;
            }
        }

        //https://www.syncfusion.com/blogs/post/struct-record-class-in-csharp.aspx
        private static void ClassRecordStruct()
        {
            #region Class - reference type

            Console.WriteLine("Class");
            Console.WriteLine("****************************");
            ClassPerson classPerson = new()
            {
                Age = 30,
                Name = "Novak Djokovic"
            };
            Console.WriteLine(classPerson.Name + " " + classPerson.Age); // Novak Djokovic 30

            var ClassPerson1 = classPerson;
            ClassPerson1.Age = 29;
            Console.WriteLine(classPerson.Name + " " + classPerson.Age); // Novak Djokovic 29
            Console.WriteLine(ClassPerson1.Name + " " + ClassPerson1.Age); // Novak Djokovic 29
            //Class is reference type, that implies both the objects will have same reference

            #endregion

            #region Struct - value type

            Console.WriteLine();
            Console.WriteLine("Struct");
            Console.WriteLine("****************************");
            StructPerson structPerson = new()
            {
                Age = 30,
                Name = "Novak Djokovic"
            };
            Console.WriteLine(structPerson.Name + " " + structPerson.Age); // Novak Djokovic 30

            var structPerson1 = structPerson;
            structPerson1.Age = 29;
            Console.WriteLine(structPerson.Name + " " + structPerson.Age); // Novak Djokovic 30
            Console.WriteLine(structPerson1.Name + " " + structPerson1.Age); // Novak Djokovic 29
            //Struct is value type, that implies changes to one object wont be impact another object

            #endregion

            #region Record - reference type/value type

            Console.WriteLine();
            Console.WriteLine("Record");
            Console.WriteLine("****************************");
            Record1Person record1Person = new("Novak Djokovic", 30);
            Console.WriteLine(record1Person.Name + " " + record1Person.Age); // Novak Djokovic 30
            //record1Person.Name = "Cannot assign";//not possible(Compiler error), values can only be assigne during initialization, individual property cannot be changed.

            var _record1Person = record1Person with { Age = 29 };
            Console.WriteLine(record1Person); // Person { Name = Novak Djokovic, Age = 30 }
            Console.WriteLine(_record1Person); // Person { Name = Novak Djokovic, Age = 29 }
            //Record is reference type that behaves identically to the value type when it relates to value equality
            //init property - cannot change value/immutable
            #endregion
        }
    }
}
