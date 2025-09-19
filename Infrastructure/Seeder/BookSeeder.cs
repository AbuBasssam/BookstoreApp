using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public class BookSeeder
{
    private static readonly Random _rand = new();
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Books.AnyAsync())
            return;
        // Disable triggers temporarily
        await context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER ALL ON BookCopies");
        await context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER ALL ON Books");
        await context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER ALL ON BookRatings");
        // Use a fixed random seed for deterministic example data
        var rnd = new Random(12345);

        // 1) Languages (31) - order must match user's list
        var languages = await context.Languages.Select(l => l.Id).ToListAsync();


        // 2) Publishers - top 10 US publishers (basic list)
        var publishers = await context.Publishers.Select(p => p.Id).ToListAsync();




        // 3) Categories (7)
        var categories = await context.Categories.Select(c => c.Id).ToListAsync();



        await context.SaveChangesAsync();

        // 4) Prepare 10 real/plausible titles per category with Arabic translations and short descriptions
        // For brevity we use realistic-sounding original titles (some inspired by popular works) and Arabic equivalents.

        // ---------------- Books (70 real titles mapped to authors) ----------------
        var seedBooks = new List<BookSeedInfo>();


        void AddBook(int id, string isbn, string titleEN, string titleAR, string descEN, string descAR, int publisherId, int authorId, int languageId, int categoryId, int pages, DateTime publishDate)
        {
            var availableDate = DateTime.UtcNow.Date.AddDays(-rnd.Next(0, 180));
            var letter = (char)('A' + (id % 26));
            var digits = (id % 90 + 10).ToString("00");
            var loc = (id % 3 == 0) ? $"{letter}{digits}-{rnd.Next(1, 999):X}" : $"{letter}{digits}";


            seedBooks.Add(new BookSeedInfo
            {
                Id = id,
                ISBN = isbn,
                TitleEN = titleEN,
                TitleAR = titleAR,
                DescriptionEN = descEN,
                DescriptionAR = descAR,
                PublisherId = publisherId,
                AuthorId = authorId,
                LanguageId = languageId,
                CategoryId = categoryId,
                Pages = pages,
                PublishDate = publishDate,
                AvailableDate = availableDate,
                Location = loc,
                LastOpenBookingsDate = null,
                IsDeleted = false,
                CoverImageUrl = $"https://10.0.2.2:7178/image/{titleEN.Replace(" ", "")}.jpg"
            });
        }
        // Romance (15)
        AddBook(1, GenerateIsbn13(1), "Pride and Prejudice", "كبرياء وتحامل", "Classic romantic novel about manners, upbringing and marriage.", "رواية رومانسية كلاسيكية عن الأخلاق والتربية والزواج.", 1, 1, 2, 1, 432, new DateTime(1813, 1, 28));
        AddBook(2, GenerateIsbn13(2), "The Notebook", "المفكرة", "A modern love story spanning decades and memories.", "قصة حب عصرية تمتد عبر عقود وذكريات.", 2, 2, 2, 1, 214, new DateTime(1996, 10, 1));
        AddBook(3, GenerateIsbn13(3), "The Bride", "العروس", "A contemporary romance exploring family and love.", "رواية رومانسية معاصرة تستكشف الأسرة والحب.", 3, 3, 2, 1, 350, new DateTime(2001, 5, 10));
        AddBook(4, GenerateIsbn13(4), "Me Before You", "أنا قبلك", "An emotional romance about love and difficult choices.", "رواية عاطفية عن الحب والخيارات الصعبة.", 4, 4, 2, 1, 369, new DateTime(2012, 1, 5));
        AddBook(5, GenerateIsbn13(5), "It Ends with Us", "ينتهي بنا المطاف", "Contemporary romance dealing with love, abuse and resilience.", "رواية رومانسية معاصرة تتعامل مع الحب والإساءة والصلابة.", 5, 5, 2, 1, 384, new DateTime(2016, 8, 2));
        AddBook(6, GenerateIsbn13(6), "Jane Eyre", "جين آير", "A classic novel of morality, religion and romance.", "رواية كلاسيكية عن الأخلاق والدين والرومانس.", 1, 6, 2, 1, 500, new DateTime(1847, 10, 16));
        AddBook(7, GenerateIsbn13(7), "Fifty Shades of Grey", "خمسون درجة من الرمادي", "Contemporary erotic romance that became a bestseller.", "رواية رومانسية إيروتيكية معاصرة أصبحت من الأكثر مبيعًا.", 2, 7, 2, 1, 514, new DateTime(2011, 6, 20));
        AddBook(8, GenerateIsbn13(8), "Chocolat", "الشوكولاتة", "A charming novel blending romance and small-town life.", "رواية ساحرة تمزج الرومانسية وحياة البلدة الصغيرة.", 3, 8, 2, 1, 352, new DateTime(1999, 9, 15));
        AddBook(9, GenerateIsbn13(9), "Outlander", "الغريب", "Historical romance with time-travel elements.", "رومانسية تاريخية مع عناصر السفر عبر الزمن.", 4, 9, 2, 1, 850, new DateTime(1991, 6, 1));
        AddBook(10, GenerateIsbn13(10), "A Court of Thorns and Roses", "محكمة الأشواك والورود", "Romantic fantasy mixing fae lore and romance.", "فانتازيا رومانسية تمزج خرافات الجان والرومانسية.", 5, 10, 2, 1, 419, new DateTime(2015, 5, 5));
        AddBook(11, GenerateIsbn13(11), "Sense and Sensibility", "عقل وعاطفة", "A story of love, romance, and heartbreak in 19th century England.", "قصة حب ورومانسية وانكسار قلب في إنجلترا القرن التاسع عشر.", 1, 1, 2, 1, 352, new DateTime(1811, 10, 30));
        AddBook(12, GenerateIsbn13(12), "A Walk to Remember", "نزهة لا تنسى", "A heartwarming story of young love and sacrifice.", "قائمة مؤثرة عن الحب الشبابي والتضحية.", 2, 2, 2, 1, 240, new DateTime(1999, 10, 1));
        AddBook(13, GenerateIsbn13(13), "Vision in White", "رؤية في الأبيض", "A contemporary romance set in the world of wedding planning.", "رومانسية معاصرة في عالم تخطيط حفلات الزفاف.", 3, 3, 2, 1, 352, new DateTime(2009, 4, 28));
        AddBook(14, GenerateIsbn13(14), "The Giver of Stars", "واهب النجوم", "A historical romance about women fighting for their rights.", "رومانسية تاريخية عن نساء يناضلن من أجل حقوقهن.", 4, 4, 2, 1, 400, new DateTime(2019, 10, 3));
        AddBook(15, GenerateIsbn13(15), "Verity", "فيريتي", "A psychological thriller with romantic elements.", "إثارة نفسية بعناصر رومانسية.", 5, 5, 2, 1, 336, new DateTime(2018, 12, 7));

        // Fantasy (15)
        AddBook(16, GenerateIsbn13(16), "The Hobbit", "الهوبيت", "A prelude to The Lord of the Rings, a classic quest.", "تمهيد للرباط، رحلة كلاسيكية.", 1, 11, 2, 2, 310, new DateTime(1937, 9, 21));
        AddBook(17, GenerateIsbn13(17), "A Game of Thrones", "صراع العروش", "Political fantasy of warring houses and dragons.", "فانتازيا سياسية عن أسر متحاربة وتنانين.", 2, 12, 2, 2, 694, new DateTime(1996, 8, 6));
        AddBook(18, GenerateIsbn13(18), "Harry Potter and the Philosopher's Stone", "هاري بوتر وحجر الفيلسوف", "A boy discovers he's a wizard and attends Hogwarts.", "صبي يكتشف أنه ساحر ويلتحق بهوغوورتس.", 3, 13, 2, 2, 223, new DateTime(1997, 6, 26));
        AddBook(19, GenerateIsbn13(19), "The Name of the Wind", "اسم الريح", "A gifted young man tells the story of his life and losses.", "شاب موهوب يروي قصة حياته وخسائره.", 4, 14, 2, 2, 662, new DateTime(2007, 3, 27));
        AddBook(20, GenerateIsbn13(20), "Mistborn: The Final Empire", "ميستبورن: الامبراطورية الأخيرة", "Epic fantasy with a unique magic system based on metals.", "فانتازيا ملحمية بنظام سحري فريد قائم على المعادن.", 5, 15, 2, 2, 672, new DateTime(2006, 7, 17));
        AddBook(21, GenerateIsbn13(21), "American Gods", "آلهة أمريكا", "A blend of modern America and ancient gods.", "مزيج من أمريكا الحديثة والآلهة القديمة.", 6, 16, 2, 2, 465, new DateTime(2001, 6, 19));
        AddBook(22, GenerateIsbn13(22), "A Wizard of Earthsea", "ساحر إيرثسي", "Classic coming-of-age fantasy in an island archipelago.", "فانتازيا كلاسيكية عن النضوج في أرخبيل جزر.", 7, 17, 2, 2, 183, new DateTime(1968, 9, 1));
        AddBook(23, GenerateIsbn13(23), "The Chronicles of Narnia: The Lion, the Witch and the Wardrobe", "أسد، ساحرة وخزانة", "Children's fantasy with moral and mythic themes.", "فانتازيا أطفال ذات مواضيع أخلاقية وأساطيرية.", 1, 18, 2, 2, 208, new DateTime(1950, 10, 16));
        AddBook(24, GenerateIsbn13(24), "Guards! Guards!", "حراس! حراس!", "Humorous fantasy from the Discworld series.", "فانتازيا هزلية من سلسلة ديسكوورلد.", 8, 19, 2, 2, 320, new DateTime(1989, 5, 1));
        AddBook(25, GenerateIsbn13(25), "Assassin's Apprentice", "متدرب القاتل", "First book of the Farseer Trilogy, an intimate fantasy told from the hero's perspective.", "الكتاب الأول من ثلاثية فارسير، فانتازيا حميمة من منظور البطل.", 9, 20, 2, 2, 400, new DateTime(1995, 6, 1));

        AddBook(26, GenerateIsbn13(26), "The Fellowship of the Ring", "رفقة الخاتم", "The first volume of The Lord of the Rings epic fantasy.", "المجلد الأول من ملحمة سيد الخواتم.", 1, 11, 2, 2, 423, new DateTime(1954, 7, 29));
        AddBook(27, GenerateIsbn13(27), "A Clash of Kings", "صدام الملوك", "The second book in A Song of Ice and Fire series.", "الكتاب الثاني في سلسلة أغنية الجليد والنار.", 2, 12, 2, 2, 761, new DateTime(1998, 11, 16));
        AddBook(28, GenerateIsbn13(28), "Harry Potter and the Chamber of Secrets", "هاري بوتر وحجرة الأسرار", "The second book in the Harry Potter series.", "الكتاب الثاني في سلسلة هاري بوتر.", 3, 13, 2, 2, 251, new DateTime(1998, 7, 2));
        AddBook(29, GenerateIsbn13(29), "The Wise Man's Fear", "خوف الرجل الحكيم", "The second book in The Kingkiller Chronicle series.", "الكتاب الثاني في سلسلة كرونيكل قاتل الملك.", 4, 14, 2, 2, 994, new DateTime(2011, 3, 1));
        AddBook(30, GenerateIsbn13(30), "The Way of Kings", "طريق الملوك", "The first book in The Stormlight Archive series.", "الكتاب الأول في سلسلة أرشيف عاصفة الضوء.", 5, 15, 2, 2, 1007, new DateTime(2010, 8, 31));

        // Mystery (15)
        AddBook(31, GenerateIsbn13(31), "Murder on the Orient Express", "جريمة في قطار الشرق السريع", "A locked-room mystery featuring Hercule Poirot.", "لغز غرفة مقفلة بشخصية هركُل بُوارو.", 1, 21, 2, 3, 256, new DateTime(1934, 1, 1));
        AddBook(32, GenerateIsbn13(32), "The Hound of the Baskervilles", "كلب باسترفيل", "Sherlock Holmes investigates a supernatural-seeming hound.", "شرلوك هولمز يحقق في كلب يبدو خارقاً.", 2, 22, 2, 3, 256, new DateTime(1902, 4, 1));
        AddBook(33, GenerateIsbn13(33), "Gone Girl", "Gone Girl", "A psychological thriller about a missing wife and media frenzy.", "إثارة نفسية عن زوجة مفقودة وهياج إعلامي.", 3, 23, 2, 3, 422, new DateTime(2012, 6, 5));
        AddBook(34, GenerateIsbn13(34), "In the Woods", "في الغابة", "A detective novel combining past trauma and a present murder investigation.", "رواية محقق تجمع بين صدمة ماضية وتحقيق جريمة حالية.", 4, 24, 2, 3, 429, new DateTime(2007, 5, 17));
        AddBook(35, GenerateIsbn13(35), "The Girl with the Dragon Tattoo", "الفتاة ذات وشم التنين", "A thriller combining mystery, family secrets and investigative journalism.", "إثارة تجمع بين الغموض وأسرار العائلة والعمل الصحفي الاستقصائي.", 5, 25, 2, 3, 465, new DateTime(2005, 8, 1));
        AddBook(36, GenerateIsbn13(36), "The Big Sleep", "النوم الكبير", "A classic hard-boiled detective novel featuring Philip Marlowe.", "رواية محقق كلاسيكية بنبرة قاسية بطليها فيليب مارلو.", 6, 26, 2, 3, 206, new DateTime(1939, 1, 1));
        AddBook(37, GenerateIsbn13(37), "The Talented Mr. Ripley", "السيد ريبلي الموهوب", "A psychological crime novel about identity and deceit.", "رواية جريمة نفسية عن الهوية والخداع.", 7, 27, 2, 3, 352, new DateTime(1955, 1, 1));
        AddBook(38, GenerateIsbn13(38), "Gaudy Night", "ليلة بهيجة", "A Lord Peter Wimsey mystery centered on a woman's college and moral questions.", "لغز لورد بيتر ويمزي يتركز على كلية نسائية ومسائل أخلاقية.", 8, 28, 2, 3, 320, new DateTime(1935, 1, 1));
        AddBook(39, GenerateIsbn13(39), "The Maltese Falcon", "الصقر المالطي", "Hard-boiled detective novel centered on a mysterious statuette.", "رواية محقق قاسية تتمحور حول تمثال غامض.", 9, 29, 2, 3, 217, new DateTime(1930, 10, 1));
        AddBook(40, GenerateIsbn13(40), "Closed Casket", "تابوت مغلق", "A modern Christie-style mystery continuation by Sophie Hannah.", "لغز حديث على طراز أغاثا كريستي بقلم صوفي هانا.", 10, 30, 2, 3, 368, new DateTime(2016, 11, 1));

        AddBook(41, GenerateIsbn13(41), "Death on the Nile", "موت على النيل", "A Hercule Poirot mystery set in Egypt.", "لغز هركول بوارو في مصر.", 1, 21, 2, 3, 288, new DateTime(1937, 11, 1));
        AddBook(42, GenerateIsbn13(42), "The Adventures of Sherlock Holmes", "مغامرات شرلوك هولمز", "A collection of Sherlock Holmes short stories.", "مجموعة من قصص شرلوك هولمز القصيرة.", 2, 22, 2, 3, 307, new DateTime(1892, 10, 14));
        AddBook(43, GenerateIsbn13(43), "Sharp Objects", "أجسام حادة", "A psychological thriller about a journalist investigating murders.", "إثارة نفسية عن صحفية تحقق في جرائم قتل.", 3, 23, 2, 3, 254, new DateTime(2006, 9, 26));
        AddBook(44, GenerateIsbn13(44), "The Trespasser", "المتعدي", "A Dublin Murder Squad mystery novel.", "رواية غموض من فرقة شرطة دبلن للقتل.", 4, 24, 2, 3, 464, new DateTime(2016, 9, 22));
        AddBook(45, GenerateIsbn13(45), "The Girl Who Played with Fire", "الفتاة التي لعبت بالنار", "The second book in the Millennium series.", "الكتاب الثاني في سلسلة الألفية.", 5, 25, 2, 3, 503, new DateTime(2006, 4, 1));

        // Programming (15)
        AddBook(46, GenerateIsbn13(46), "Clean Code", "كود نظيف", "A handbook of agile software craftsmanship focusing on writing maintainable code.", "دليل لحرفية تطوير البرامج يركز على كتابة كود سهل الصيانة.", 7, 31, 2, 4, 464, new DateTime(2008, 8, 1));
        AddBook(47, GenerateIsbn13(47), "The Pragmatic Programmer", "المبرمج العملي", "Practical advice for software developers to improve craftsmanship and productivity.", "نصائح عملية لمطوري البرمجيات لتحسين الحرفية والإنتاجية.", 8, 32, 2, 4, 352, new DateTime(1999, 10, 30));
        AddBook(48, GenerateIsbn13(48), "Design Patterns", "أنماط التصميم", "Classic catalog of software design patterns by the Gang of Four.", "فهرس كلاسيكي لأنماط تصميم البرمجيات من مجموعة الأربعة.", 9, 34, 2, 4, 395, new DateTime(1994, 10, 21));
        AddBook(49, GenerateIsbn13(49), "C# in Depth", "C# بعمق", "Deep dive into modern C# language features and idioms.", "نظرة عميقة على ميزات وعبارات لغة C# الحديثة.", 1, 35, 2, 4, 520, new DateTime(2019, 3, 23));
        AddBook(50, GenerateIsbn13(50), "Refactoring", "إعادة هيكلة الكود", "Improving the design of existing code with catalog of refactorings.", "تحسين تصميم الكود القائم مع فهرس لإعادة الهيكلة.", 2, 36, 2, 4, 448, new DateTime(1999, 7, 8));
        AddBook(51, GenerateIsbn13(51), "CLR via C#", "CLR عبر C#", "In-depth coverage of the .NET CLR internals and advanced C# topics.", "تغطية متعمقة لداخلية CLR في .NET ومواضيع متقدمة في C#.", 3, 37, 2, 4, 900, new DateTime(2012, 10, 1));
        AddBook(52, GenerateIsbn13(52), "Introduction to Algorithms", "مقدمة في الخوارزميات", "Comprehensive algorithms textbook used in academia and industry.", "كتاب شامل عن الخوارزميات يستخدم في الأكاديميا والصناعة.", 4, 38, 2, 4, 1312, new DateTime(1990, 9, 1));
        AddBook(53, GenerateIsbn13(53), "You Don't Know JS", "أنت لا تعرف جافا سكريبت", "A deep series on JavaScript internals for serious JS developers.", "سلسلة متعمقة عن باطن جافا سكريبت للمطورين الجادين.", 5, 39, 2, 4, 250, new DateTime(2014, 1, 1));
        AddBook(54, GenerateIsbn13(54), "Designing Data-Intensive Applications", "تصميم تطبيقات كثيفة البيانات", "Patterns and principles for building reliable, scalable data systems.", "أنماط ومبادئ لبناء أنظمة بيانات موثوقة وقابلة للتوسع.", 6, 40, 2, 4, 616, new DateTime(2017, 3, 15));
        AddBook(55, GenerateIsbn13(55), "Domain-Driven Design", "تصميم موجه المجال", "Tactical and strategic patterns for designing complex software systems.", "أنماط تكتيكية واستراتيجية لتصميم أنظمة برمجية معقدة.", 7, 36, 2, 4, 560, new DateTime(2003, 8, 30));

        AddBook(56, GenerateIsbn13(56), "Code Complete", "اكتمال الكود", "A practical handbook of software construction.", "دليل عملي لبناء البرمجيات.", 7, 31, 2, 4, 960, new DateTime(1993, 5, 1));
        AddBook(57, GenerateIsbn13(57), "The Clean Coder", "المبرمج النظيف", "A code of conduct for professional programmers.", "مدونة سلوك للمبرمجين المحترفين.", 8, 32, 2, 4, 256, new DateTime(2011, 5, 13));
        AddBook(58, GenerateIsbn13(58), "Head First Design Patterns", "أنماط التصميم بطريقة سهلة", "A brain-friendly guide to design patterns.", "دليل سهل لأنماط التصميم.", 9, 34, 2, 4, 694, new DateTime(2004, 10, 25));
        AddBook(59, GenerateIsbn13(59), "C# 9.0 in a Nutshell", "C# 9.0 باختصار", "The definitive reference for C# 9.0.", "المرجع النهائي لـ C# 9.0.", 1, 35, 2, 4, 1056, new DateTime(2021, 3, 1));
        AddBook(60, GenerateIsbn13(60), "Working Effectively with Legacy Code", "العمل بفعالية مع الكود القديم", "Techniques for maintaining and improving legacy systems.", "تقنيات للحفاظ على وتحسين الأنظمة القديمة.", 2, 36, 2, 4, 456, new DateTime(2004, 9, 1));


        // Economics (15)
        AddBook(61, GenerateIsbn13(61), "The Return of Depression Economics", "عودة اقتصاد الكساد", "Paul Krugman's analysis of macroeconomic failures and crises.", "تحليل بول كروغمان لفشل الأوضاع الاقتصادية والأزمات.", 1, 41, 2, 5, 320, new DateTime(1999, 1, 1));
        AddBook(62, GenerateIsbn13(62), "Capital in the Twenty-First Century", "رأس المال في القرن الحادي والعشرين", "Piketty's comprehensive study of wealth and inequality.", "دراسة شاملة لبيكيتي عن الثروة واللامساواة.", 2, 42, 2, 5, 696, new DateTime(2013, 4, 14));
        AddBook(63, GenerateIsbn13(63), "Principles of Economics", "مبادئ الاقتصاد", "Foundational economics textbook covering micro and macro topics.", "كتاب دراسي تأسيسي يغطي مواضيع الاقتصاد الجزئي والكلي.", 3, 43, 2, 5, 720, new DateTime(2014, 1, 1));
        AddBook(64, GenerateIsbn13(64), "Capitalism and Freedom", "الرأسمالية والحرية", "Milton Friedman's classic defense of free-market principles.", "دفاع ميلتون فريدمان الكلاسيكي عن مبادئ السوق الحرة.", 4, 44, 2, 5, 256, new DateTime(1962, 4, 1));
        AddBook(65, GenerateIsbn13(65), "Development as Freedom", "التنمية كحرية", "Amartya Sen's influential work linking development and human freedom.", "عمل أمارتيا سين المؤثر الذي يربط التنمية بالحرية الإنسانية.", 5, 45, 2, 5, 352, new DateTime(1999, 9, 1));
        AddBook(66, GenerateIsbn13(66), "The Price of Inequality", "سعر اللامساواة", "Joseph Stiglitz on how inequality undermines economies and democracy.", "جوزيف ستيغليتز عن كيف تقوض اللامساواة الاقتصادات والديمقراطية.", 6, 46, 2, 5, 288, new DateTime(2012, 9, 1));
        AddBook(67, GenerateIsbn13(67), "Why Nations Fail", "لماذا تفشل الدول", "Acemoglu and Robinson explore political and economic institutions shaping prosperity.", "عجم أوغلو وروبينسون يستكشفان المؤسسات السياسية والاقتصادية التي تشكل الازدهار.", 7, 47, 2, 5, 544, new DateTime(2012, 3, 1));
        AddBook(68, GenerateIsbn13(68), "Bad Samaritans", "السامريون الأشرار", "Ha-Joon Chang critiques free-market policies and development orthodoxy.", "ها جون تشانغ ينتقد سياسات السوق الحرة وأرثوذكسية التنمية.", 8, 48, 2, 5, 320, new DateTime(2008, 11, 1));
        AddBook(69, GenerateIsbn13(69), "Thinking, Fast and Slow", "التفكير، سريع وبطيء", "Kahneman's exploration of the two systems of thought and judgment.", "استكشاف كانيمان لنظامي التفكير والحكم.", 9, 49, 2, 5, 499, new DateTime(2011, 10, 25));
        AddBook(70, GenerateIsbn13(70), "The Great Escape: Health, Wealth, and the Origins of Inequality", "الهروب العظيم: الصحة والثروة وأصول اللامساواة", "Angus Deaton on global health, wealth and inequality trends.", "أغنس دياتون عن الصحة العالمية والثروة واتجاهات اللامساواة.", 10, 50, 2, 5, 560, new DateTime(2013, 12, 1));
        AddBook(71, GenerateIsbn13(71), "Freakonomics", "فريكونوميكس", "A rogue economist explores the hidden side of everything.", "خبير اقتصادي متمرد يستكشف الجانب الخفي لكل شيء.", 1, 41, 2, 5, 320, new DateTime(2005, 4, 12));
        AddBook(72, GenerateIsbn13(72), "The Wealth of Nations", "ثروة الأمم", "The foundational work of modern economics.", "العمل التأسيسي للاقتصاد الحديث.", 2, 42, 2, 5, 950, new DateTime(1776, 3, 9));
        AddBook(73, GenerateIsbn13(73), "Nudge", "دفعة", "Improving decisions about health, wealth, and happiness.", "تحسين القرارات حول الصحة والثروة والسعادة.", 3, 43, 2, 5, 320, new DateTime(2008, 4, 8));
        AddBook(74, GenerateIsbn13(74), "The Undercover Economist", "الخبير الاقتصادي السري", "Revealing the economic principles behind everyday life.", "كشف المبادئ الاقتصادية وراء الحياة اليومية.", 4, 44, 2, 5, 288, new DateTime(2005, 11, 3));
        AddBook(75, GenerateIsbn13(75), "The Economic Consequences of the Peace", "العواقب الاقتصادية للسلام", "Keynes' critique of the Treaty of Versailles.", "نقد كينز لمعاهدة فرساي.", 5, 45, 2, 5, 320, new DateTime(1919, 12, 1));

        // History (15)
        AddBook(76, GenerateIsbn13(76), "The Age of Revolution", "عصر الثورة", "Eric Hobsbawm's study of the revolutionary era from 1789 to 1848.", "دراسة إيريك هوبزباوم لعصر الثورة 1789-1848.", 1, 51, 2, 6, 640, new DateTime(1962, 1, 1));
        AddBook(77, GenerateIsbn13(77), "Citizens: A Chronicle of the French Revolution", "المواطنون: كرونیکل الثورة الفرنسية", "Simon Schama's narrative history of the French Revolution.", "سرد تاريخي لسيمون شاما عن الثورة الفرنسية.", 2, 52, 2, 6, 704, new DateTime(1989, 1, 1));
        AddBook(78, GenerateIsbn13(78), "The Guns of August", "بوارق أغسطس", "Barbara Tuchman's narrative of the outbreak of World War I.", "سرد باربرا توكمان لبداية الحرب العالمية الأولى.", 3, 53, 2, 6, 511, new DateTime(1962, 1, 1));
        AddBook(79, GenerateIsbn13(79), "The Ascent of Money", "صعود المال", "Niall Ferguson's history of finance and economic institutions.", "تاريخ التمويل والمؤسسات الاقتصادية لنيل فيرغسون.", 4, 54, 2, 6, 480, new DateTime(2008, 1, 1));
        AddBook(80, GenerateIsbn13(80), "SPQR: A History of Ancient Rome", "SPQR: تاريخ روما القديمة", "Mary Beard's accessible account of Rome's institutions and society.", "سرد ماري بيرد الميسر لمؤسسات ومجتمع روما.", 5, 55, 2, 6, 512, new DateTime(2015, 1, 1));
        AddBook(81, GenerateIsbn13(81), "1776", "1776", "David McCullough's narrative history of the American Revolution year.", "السرد التاريخي لديفيد ماكولو لعام الثورة الأمريكية.", 6, 56, 2, 6, 386, new DateTime(2005, 5, 24));
        AddBook(82, GenerateIsbn13(82), "Guns, Germs, and Steel", "الأسلحة والجراثيم والصلب", "Jared Diamond's environmental explanation for global inequalities.", "تفسير جاريد دايموند البيئي لللامساواة العالمية.", 7, 57, 2, 6, 528, new DateTime(1997, 3, 1));
        AddBook(83, GenerateIsbn13(83), "A History of Warfare", "تاريخ الحرب", "John Keegan's broad survey of military history and human conflict.", "استعراض جون كيغان الواسع لتاريخ الحرب والصراع البشري.", 8, 58, 2, 6, 496, new DateTime(1993, 1, 1));
        AddBook(84, GenerateIsbn13(84), "Rubicon: The Last Years of the Roman Republic", "روبيكون: السنوات الأخيرة للجمهورية الرومانية", "Tom Holland's account of Rome's transition from republic to empire.", "سرد توم هولاند لتحول روما من جمهورية إلى إمبراطورية.", 9, 59, 2, 6, 560, new DateTime(2003, 5, 1));
        AddBook(85, GenerateIsbn13(85), "Stalingrad", "ستالينغراد", "Antony Beevor's detailed account of the Battle of Stalingrad.", "سرد مفصل لأنتوني بيفور لمعركة ستالينغراد.", 10, 60, 2, 6, 576, new DateTime(1998, 9, 1));
        AddBook(86, GenerateIsbn13(86), "The Age of Extremes", "عصر التطرف", "Eric Hobsbawm's history of the short twentieth century.", "تاريخ إيريك هوبزباوم للقرن العشرين القصير.", 1, 51, 2, 6, 640, new DateTime(1994, 1, 1));
        AddBook(87, GenerateIsbn13(87), "The History of the Ancient World", "تاريخ العالم القديم", "A comprehensive history of ancient civilizations.", "تاريخ شامل للحضارات القديمة.", 2, 52, 2, 6, 896, new DateTime(2007, 3, 1));
        AddBook(88, GenerateIsbn13(88), "The Second World War", "الحرب العالمية الثانية", "A comprehensive history of WWII.", "تاريخ شامل للحرب العالمية الثانية.", 3, 53, 2, 6, 720, new DateTime(1989, 1, 1));
        AddBook(89, GenerateIsbn13(89), "The Silk Roads", "طرق الحرير", "A new history of the world from the perspective of the Silk Roads.", "تاريخ جديد للعالم من منظور طرق الحرير.", 4, 54, 2, 6, 656, new DateTime(2015, 8, 27));
        AddBook(90, GenerateIsbn13(90), "The Crusades", "الحروب الصليبية", "A comprehensive history of the Crusades.", "تاريخ شامل للحروب الصليبية.", 5, 55, 2, 6, 784, new DateTime(2012, 9, 1));

        // Science (15)
        AddBook(91, GenerateIsbn13(91), "Cosmos", "الكون", "Carl Sagan explores the universe, science, and how humans relate to cosmos.", "كارل ساجان يستكشف الكون والعلم وكيف يتصل البشر بالكون.", 1, 61, 2, 7, 384, new DateTime(1980, 10, 12));
        AddBook(92, GenerateIsbn13(92), "A Brief History of Time", "تاريخ موجز للزمن", "Stephen Hawking discusses black holes, Big Bang, and nature of time.", "ستيفن هوكينغ يناقش الثقوب السوداء، الانفجار العظيم، وطبيعة الزمن.", 2, 62, 2, 7, 256, new DateTime(1988, 4, 1));
        AddBook(93, GenerateIsbn13(93), "The Selfish Gene", "الجين الأناني", "Richard Dawkins presents gene-centered view of evolution.", "ريتشارد دوكنز يعرض منظور الجين في التطور.", 3, 63, 2, 7, 360, new DateTime(1976, 3, 13));
        AddBook(94, GenerateIsbn13(94), "The Immortal Life of Henrietta Lacks", "الحياة الخالدة لهينريتا لاكس", "Rebecca Skloot examines the life of Henrietta Lacks and medical ethics.", "ريبيكا سكولوت تفحص حياة هنريتا لاكس وأخلاقيات الطب.", 4, 64, 2, 7, 381, new DateTime(2010, 2, 2));
        AddBook(95, GenerateIsbn13(95), "The Gene: An Intimate History", "الجين: تاريخ حميمي", "Siddhartha Mukherjee traces the history and science of the gene.", "سيدهارتا موكيرجي يرصد تاريخ الجين والعلوم المرتبطة به.", 5, 65, 2, 7, 592, new DateTime(2016, 5, 17));
        AddBook(96, GenerateIsbn13(96), "Silent Spring", "الربيع الصامت", "Rachel Carson's critique of pesticides and impact on the environment.", "راشيل كارسون تنتقد المبيدات وتأثيرها على البيئة.", 6, 66, 2, 7, 368, new DateTime(1962, 9, 27));
        AddBook(97, GenerateIsbn13(97), "The Structure of Scientific Revolutions", "بنية الثورات العلمية", "Thomas S. Kuhn's study on paradigm shifts in scientific thought.", "توماس س. كون دراسة حول التحولات النظرية في الفكر العلمي.", 7, 67, 2, 7, 264, new DateTime(1962, 1, 1));
        AddBook(98, GenerateIsbn13(98), "Pale Blue Dot", "النقطة الزرقاء الباهتة", "Carl Sagan reflects on Earth's place in the universe.", "كارل ساجان يتأمل مكان الأرض في الكون.", 8, 61, 2, 7, 256, new DateTime(1994, 2, 1));
        AddBook(99, GenerateIsbn13(99), "The Double Helix", "الحلزون المزدوج", "James Watson's personal account of the discovery of the structure of DNA.", "رواية جيمس واتسون الشخصية لاكتشاف بنية الحمض النووي.", 9, 70, 2, 7, 226, new DateTime(1968, 2, 26));

        AddBook(100, GenerateIsbn13(100), "Sapiens: A Brief History of Humankind", "الإنسان: تاريخ مختصر للبشرية", "Yuval Noah Harari narrates history of humankind from ancient to modern.", "يوفال نوح هراري يروي تاريخ البشرية من القديم إلى الحديث.", 10, 69, 2, 7, 498, new DateTime(2011, 1, 1));
        AddBook(101, GenerateIsbn13(101), "The Demon-Haunted World", "العالم المسكون بالشياطين", "Science as a candle in the dark.", "العلم كشمعة في الظلام.", 1, 61, 2, 7, 480, new DateTime(1995, 2, 1));
        AddBook(102, GenerateIsbn13(102), "The Grand Design", "التصميم العظيم", "Stephen Hawking's final theory of the universe.", "نظرية ستيفن هوكينغ النهائية للكون.", 2, 62, 2, 7, 208, new DateTime(2010, 9, 7));
        AddBook(103, GenerateIsbn13(103), "The Blind Watchmaker", "صانع الساعات الأعمى", "Why the evidence of evolution reveals a universe without design.", "لماذا يكشف دليل التطور عن كون بلا تصميم.", 3, 63, 2, 7, 352, new DateTime(1986, 1, 1));
        AddBook(104, GenerateIsbn13(104), "The Emperor of All Maladies", "إمبراطور كل الأمراض", "A biography of cancer.", "سيرة ذاتية للسرطان.", 4, 64, 2, 7, 592, new DateTime(2010, 11, 16));
        AddBook(105, GenerateIsbn13(105), "The Order of Time", "ترتيب الزمن", "Carlo Rovelli's exploration of the nature of time.", "استكشاف كارلو روفيللي لطبيعة الزمن.", 5, 65, 2, 7, 240, new DateTime(2017, 5, 1));
        // Map seedBooks to Book entities and insert
        var books = seedBooks.Select(b => new Book
        {
            ISBN = b.ISBN,
            TitleEN = b.TitleEN,
            TitleAR = b.TitleAR,
            DescriptionEN = b.DescriptionEN,
            DescriptionAR = b.DescriptionAR,
            PublisherID = b.PublisherId,
            AuthorID = b.AuthorId,
            LanguageID = b.LanguageId,
            CategoryID = (enCategory)b.CategoryId,
            PageCount = (short)b.Pages,
            PublishDate = b.PublishDate,
            AvailabilityDate = DateOnly.Parse(b.AvailableDate.Date.ToShortDateString()),
            Position = b.Location,
            LastWaitListOpenDate = null,
            IsActive = !b.IsDeleted,
            CoverImage = b.CoverImageUrl
        }).ToList();

        await context.Books.AddRangeAsync(books);

        await context.SaveChangesAsync();

        // 7) Copies for each book (1-5 copies)
        var copies = new List<BookCopy>();
        foreach (var bk in seedBooks)
        {
            var copiesCount = 1 + (bk.Id % 5); // 1..5
            for (int c = 0; c < copiesCount; c++)
            {
                copies.Add(new BookCopy
                {
                    BookID = bk.Id,
                    IsAvailable = (c % 3 != 0),
                    IsOnHold = (c % 4 == 0)
                });
            }
        }

        await context.BookCopies.AddRangeAsync(copies);

        await context.SaveChangesAsync();

        // 8) Borrow records for copies by members with IDs 2..6
        var borrowRecords = new List<BorrowingRecord>();
        var members = new[] { 2, 3, 4, 5, 6 };

        foreach (var cp in copies)
        {
            // generate 0..8 borrows per copy, more for some books to create popularity
            var numBorrows = (cp.BookID % 7) + (cp.Id % 3); // varied
            for (int bidx = 0; bidx < numBorrows; bidx++)
            {
                var daysAgoStart = rnd.Next(1, 180);
                var start = DateTime.UtcNow.Date.AddDays(-daysAgoStart);
                var duration = 7 + rnd.Next(0, 30);
                var end = start.AddDays(duration);
                var returned = (rnd.NextDouble() > 0.1) ? end.AddDays(-rnd.Next(0, 5)) : (DateTime?)null; // some not returned

                borrowRecords.Add(new BorrowingRecord
                {
                    BookCopyID = cp.Id,
                    MemberID = members[rnd.Next(members.Length)],
                    ReservationRecordID = null,
                    BorrowingDate = start,
                    DueDate = end,
                    ReturnDate = returned,
                    RenewalCount = (byte)rnd.Next(0, 3),
                    AdminID = 1
                });
            }
        }

        if (!await context.Set<BorrowingRecord>().AnyAsync())
            await context.Set<BorrowingRecord>().AddRangeAsync(borrowRecords);

        await context.SaveChangesAsync();

        // 9) Ratings: create ratings for some books by members 2..6
        var ratings = new List<BookRating>();
        foreach (var b in seedBooks)
        {
            var rCount = (b.Id % 5); // 0..4 ratings
            for (int r = 0; r < rCount; r++)
            {
                ratings.Add(new BookRating
                {
                    //Id = ratingId++,
                    BookID = b.Id,
                    UserID = members[(b.Id + r) % members.Length],
                    Rating = (byte)(1 + (rnd.Next(5))),
                });
            }
        }

        if (!await context.Set<BookRating>().AnyAsync())
            await context.Set<BookRating>().AddRangeAsync(ratings);

        await context.SaveChangesAsync();
    }

    // ----------------- Helper types and methods -----------------

    private static string GenerateIsbn13(int seed)
    {
        // simple deterministic 13-digit number for example purposes
        var baseNum = 9780000000000L + seed * 111;
        return baseNum.ToString();
    }

    private class BookSeedInfo
    {
        public int Id;
        public string ISBN;
        public string TitleEN;
        public string TitleAR;
        public string DescriptionEN;
        public string DescriptionAR;
        public int PublisherId;
        public int AuthorId;
        public int LanguageId;
        public int CategoryId;
        public int Pages;
        public DateTime PublishDate;
        public DateTime AvailableDate;
        public string Location;
        public DateTime? LastOpenBookingsDate;
        public bool IsDeleted;
        public string CoverImageUrl;
    }
}
