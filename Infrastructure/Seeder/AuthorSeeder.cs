using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class AuthorSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Authors.AnyAsync())
            return;
        var authors = new List<Author>
        {
            // Romance
            new Author{ NameEN="Jane Austen", NameAR="جين أوستن", BirthDate=new DateTime(1775,12,16), Bio=Truncate("English novelist known for her social commentary and romantic fiction, author of 'Pride and Prejudice'.",200)},
            new Author{ NameEN="Nicholas Sparks", NameAR="نيكولاس باركس", BirthDate=new DateTime(1965,12,31), Bio=Truncate("American novelist known for romantic novels often set in North Carolina, author of 'The Notebook'.",200)},
            new Author{ NameEN="Nora Roberts", NameAR="نورا روبرتس", BirthDate=new DateTime(1950,10,10), Bio=Truncate("Prolific American romance writer with numerous bestselling novels across romance and fantasy genres.",200)},
            new Author{ NameEN="Jojo Moyes", NameAR="جوجو مويس", BirthDate=new DateTime(1969,8,4), Bio=Truncate("English novelist and journalist, best known for 'Me Before You' and emotionally-driven contemporary romances.",200)},
            new Author{ NameEN="Colleen Hoover", NameAR="كولين هوفر", BirthDate=new DateTime(1979,12,11), Bio=Truncate("American author of contemporary romance and young adult fiction, known for hit titles and strong reader engagement.",200)},
            new Author{ NameEN="Charlotte Brontë", NameAR="شارلوت برونتي", BirthDate=new DateTime(1816,4,21), Bio=Truncate("English novelist and poet, author of 'Jane Eyre', noted for themes of morality and social criticism.",200)},
            new Author{ NameEN="E. L. James", NameAR="إي. إل. جيمس", BirthDate=new DateTime(1963,3,7), Bio=Truncate("British author best known for the 'Fifty Shades' series, blending contemporary romance and erotica.",200)},
            new Author{ NameEN="Joanne Harris", NameAR="جوان هاريس", BirthDate=new DateTime(1964,11,3), Bio=Truncate("English writer of novels blending romance, folklore and food; author of 'Chocolat'.",200)},
            new Author{ NameEN="Diana Gabaldon", NameAR="ديانا جابالدون", BirthDate=new DateTime(1952,1,11), Bio=Truncate("American author known for the 'Outlander' series that mixes historical fiction and romance.",200)},
            new Author{ NameEN="Sarah J. Maas", NameAR="سارة جيه. ماس", BirthDate=new DateTime(1986,3,5), Bio=Truncate("American fantasy author whose works often combine romance and epic fantasy elements.",200)},

            // Fantasy
            new Author{ NameEN="J.R.R. Tolkien", NameAR="ج. ر. ر. تولكين", BirthDate=new DateTime(1892,1,3), Bio=Truncate("English writer and philologist, best known for 'The Lord of the Rings' and worldbuilding in Middle-earth.",200)},
            new Author{ NameEN="George R. R. Martin", NameAR="جورج ر. ر. مارتن", BirthDate=new DateTime(1948,9,20), Bio=Truncate("American novelist and short story writer in fantasy and science fiction, author of 'A Song of Ice and Fire'.",200)},
            new Author{ NameEN="J.K. Rowling", NameAR="ج. ك. رولينغ", BirthDate=new DateTime(1965,7,31), Bio=Truncate("British author best known for the 'Harry Potter' series, which blends fantasy, coming-of-age and adventure.",200)},
            new Author{ NameEN="Patrick Rothfuss", NameAR="باتريك روثفوس", BirthDate=new DateTime(1973,6,6), Bio=Truncate("American writer of epic fantasy, author of 'The Name of the Wind'.",200)},
            new Author{ NameEN="Brandon Sanderson", NameAR="براندون ساندرسون", BirthDate=new DateTime(1975,12,19), Bio=Truncate("American fantasy and science fiction author known for complex magic systems and prolific output.",200)},
            new Author{ NameEN="Neil Gaiman", NameAR="نيل جايمان", BirthDate=new DateTime(1960,11,10), Bio=Truncate("English author of novels, comics and films, blending myth, fantasy and modern storytelling.",200)},
            new Author{ NameEN="Ursula K. Le Guin", NameAR="أورسولا ك. لو جوين", BirthDate=new DateTime(1929,10,21), Bio=Truncate("American author of science fiction and fantasy exploring sociological and anthropological themes.",200)},
            new Author{ NameEN="C.S. Lewis", NameAR="سي. إس. لويس", BirthDate=new DateTime(1898,11,29), Bio=Truncate("British writer and scholar, author of 'The Chronicles of Narnia' and works on Christian apologetics.",200)},
            new Author{ NameEN="Terry Pratchett", NameAR="تيري براتشيت", BirthDate=new DateTime(1948,4,28), Bio=Truncate("English humorist and fantasy author best known for the 'Discworld' series blending satire and fantasy.",200)},
            new Author{ NameEN="Robin Hobb", NameAR="روبن هوب", BirthDate=new DateTime(1952,3,5), Bio=Truncate("American fantasy author known for character-driven epic fantasy series like 'The Farseer Trilogy'.",200)},

            // Mystery
            new Author{ NameEN="Agatha Christie", NameAR="أغاثا كريستي", BirthDate=new DateTime(1890,9,15), Bio=Truncate("English writer known as the 'Queen of Crime' for her detective novels featuring Poirot and Marple.",200)},
            new Author{ NameEN="Arthur Conan Doyle", NameAR="أرثر كونان دويل", BirthDate=new DateTime(1859,5,22), Bio=Truncate("British writer best known for creating Sherlock Holmes and pioneering detective fiction.",200)},
            new Author{ NameEN="Gillian Flynn", NameAR="جيليان فلين", BirthDate=new DateTime(1971,2,24), Bio=Truncate("American writer known for psychological thrillers like 'Gone Girl' with unreliable narrators.",200)},
            new Author{ NameEN="Tana French", NameAR="تانا فرينش", BirthDate=new DateTime(1973,12,10), Bio=Truncate("Irish novelist and actor known for literary psychological crime novels set in Dublin.",200)},
            new Author{ NameEN="Stieg Larsson", NameAR="ستيغ لارسن", BirthDate=new DateTime(1954,8,15), Bio=Truncate("Swedish journalist and writer best known for the 'Millennium' series featuring Lisbeth Salander.",200)},
            new Author{ NameEN="Raymond Chandler", NameAR="ريموند تشاندلر", BirthDate=new DateTime(1888,7,23), Bio=Truncate("American-British novelist and screenwriter who shaped the hard-boiled detective genre.",200)},
            new Author{ NameEN="Patricia Highsmith", NameAR="باتريشيا هايسميث", BirthDate=new DateTime(1921,1,19), Bio=Truncate("American novelist known for psychological thrillers like 'The Talented Mr. Ripley'.",200)},
            new Author{ NameEN="Dorothy L. Sayers", NameAR="دوروثي إل. سايرز", BirthDate=new DateTime(1893,6,13), Bio=Truncate("English crime writer and scholar, creator of detective Lord Peter Wimsey.",200)},
            new Author{ NameEN="Dashiell Hammett", NameAR="داشيل هاميت", BirthDate=new DateTime(1894,5,27), Bio=Truncate("American author of hard-boiled detective novels like 'The Maltese Falcon'.",200)},
            new Author{ NameEN="Sophie Hannah", NameAR="صوفي هانا", BirthDate=new DateTime(1971,1,1), Bio=Truncate("British poet and novelist known for psychological crime fiction and continuation novels of Agatha Christie.",200)},

            // Programming
            new Author{ NameEN="Robert C. Martin", NameAR="روبرت سي. مارتن", BirthDate=new DateTime(1952,12,5), Bio=Truncate("Software engineer and author, known as 'Uncle Bob', author of 'Clean Code' and agile thought leader.",200)},
            new Author{ NameEN="Andrew Hunt", NameAR="أندرو هنت", BirthDate=new DateTime(1964,1,1), Bio=Truncate("Co-author of 'The Pragmatic Programmer' and contributor to pragmatic software development practices.",200)},
            new Author{ NameEN="David Thomas", NameAR="ديفيد توماس", BirthDate=new DateTime(1956,1,1), Bio=Truncate("Co-author of 'The Pragmatic Programmer' and influential software developer and writer.",200)},
            new Author{ NameEN="Erich Gamma", NameAR="إريش جاما", BirthDate=new DateTime(1961,3,13), Bio=Truncate("Software engineer and one of the 'Gang of Four' authors of 'Design Patterns'.",200)},
            new Author{ NameEN="Jon Skeet", NameAR="جون سكيت", BirthDate=new DateTime(1976,1,1), Bio=Truncate("Software engineer and prolific author focused on C# and .NET, known for 'C# in Depth'.",200)},
            new Author{ NameEN="Martin Fowler", NameAR="مارتن فاولر", BirthDate=new DateTime(1963,12,18), Bio=Truncate("British software engineer and author on software design, refactoring and architecture.",200)},
            new Author{ NameEN="Jeffrey Richter", NameAR="جيفري ريتشر", BirthDate=new DateTime(1955,1,1), Bio=Truncate("Author of in-depth Windows and .NET programming books including 'CLR via C#'.",200)},
            new Author{ NameEN="Thomas H. Cormen", NameAR="توماس ه. كورمن", BirthDate=new DateTime(1956,1,1), Bio=Truncate("Computer scientist and co-author of 'Introduction to Algorithms', a standard algorithms textbook.",200)},
            new Author{ NameEN="Kyle Simpson", NameAR="كايل سيمبسون", BirthDate=new DateTime(1985,1,1), Bio=Truncate("JavaScript educator and author of the 'You Don't Know JS' book series.",200)},
            new Author{ NameEN="Martin Kleppmann", NameAR="مارتن كليبمان", BirthDate=new DateTime(1986,1,1), Bio=Truncate("Researcher and author of 'Designing Data-Intensive Applications' on building reliable systems.",200)},

            // Economics
            new Author{ NameEN="Paul Krugman", NameAR="بول كروغمان", BirthDate=new DateTime(1953,2,28), Bio=Truncate("American economist, Nobel laureate, and author of books on international economics and public policy.",200)},
            new Author{ NameEN="Thomas Piketty", NameAR="توماس بيكيتي", BirthDate=new DateTime(1971,5,7), Bio=Truncate("French economist best known for 'Capital in the Twenty-First Century' on wealth and inequality.",200)},
            new Author{ NameEN="N. Gregory Mankiw", NameAR="ن. غريغوري مانكيو", BirthDate=new DateTime(1958,2,3), Bio=Truncate("American economist and author of widely-used principles of economics textbooks.",200)},
            new Author{ NameEN="Milton Friedman", NameAR="ميلتون فريدمان", BirthDate=new DateTime(1912,7,31), Bio=Truncate("Influential American economist and Nobel laureate, advocate of free-market policies.",200)},
            new Author{ NameEN="Amartya Sen", NameAR="أمارتيا سين", BirthDate=new DateTime(1933,11,3), Bio=Truncate("Indian economist and philosopher known for work on welfare economics and development.",200)},
            new Author{ NameEN="Joseph E. Stiglitz", NameAR="جوزيف ستيغليتز", BirthDate=new DateTime(1943,2,9), Bio=Truncate("American economist and Nobel laureate noted for work on information asymmetry and public policy.",200)},
            new Author{ NameEN="Daron Acemoglu", NameAR="دارون عجم أوغلو", BirthDate=new DateTime(1967,9,3), Bio=Truncate("Turkish-American economist known for work on political economy and institutions, co-author of 'Why Nations Fail'.",200)},
            new Author{ NameEN="Ha-Joon Chang", NameAR="ها جون تشانغ", BirthDate=new DateTime(1963,8,7), Bio=Truncate("South Korean economist critical of free-market orthodoxy and author on development policy.",200)},
            new Author{ NameEN="Daniel Kahneman", NameAR="دانيال كانيمان", BirthDate=new DateTime(1934,3,5), Bio=Truncate("Israeli-American psychologist and economist, Nobel laureate for work on prospect theory and decision-making.",200)},
            new Author{ NameEN="Angus Deaton", NameAR="أغنس دياتون", BirthDate=new DateTime(1945,10,19), Bio=Truncate("British-American economist awarded Nobel for analyses of consumption, poverty and welfare.",200)},

            // History
            new Author{ NameEN="Eric Hobsbawm", NameAR="إيريك هوبزباوم", BirthDate=new DateTime(1917,6,9), Bio=Truncate("British historian of the 19th and 20th centuries, author of 'The Age of Extremes'.",200)},
            new Author{ NameEN="Simon Schama", NameAR="سيمون شاما", BirthDate=new DateTime(1945,2,13), Bio=Truncate("British historian and author with works on art, history and national identity.",200)},
            new Author{ NameEN="Barbara W. Tuchman", NameAR="باربرا توكمان", BirthDate=new DateTime(1912,1,30), Bio=Truncate("American historian and author known for narrative histories like 'The Guns of August'.",200)},
            new Author{ NameEN="Niall Ferguson", NameAR="نيل فيرغسون", BirthDate=new DateTime(1964,4,18), Bio=Truncate("British historian specializing in financial and imperial history and grand historical narratives.",200)},
            new Author{ NameEN="Mary Beard", NameAR="ماري بيرد", BirthDate=new DateTime(1955,1,1), Bio=Truncate("British classicist and public intellectual known for accessible books on Roman history.",200)},
            new Author{ NameEN="David McCullough", NameAR="ديفيد ماكولو", BirthDate=new DateTime(1933,7,7), Bio=Truncate("American author and historian known for narrative biographies and US history accounts.",200)},
            new Author{ NameEN="Jared Diamond", NameAR="جاريد دايموند", BirthDate=new DateTime(1937,9,10), Bio=Truncate("American scientist and author of 'Guns, Germs, and Steel' exploring geographic and environmental factors in history.",200)},
            new Author{ NameEN="John Keegan", NameAR="جون كيغان", BirthDate=new DateTime(1934,5,15), Bio=Truncate("British military historian and author of accessible books on warfare and its human dimensions.",200)},
            new Author{ NameEN="Tom Holland", NameAR="توم هولاند", BirthDate=new DateTime(1968,1,5), Bio=Truncate("English historian and author known for engaging narratives on ancient and medieval history.",200)},
            new Author{ NameEN="Antony Beevor", NameAR="أنتوني بيفور", BirthDate=new DateTime(1946,8,14), Bio=Truncate("British military historian acclaimed for detailed, readable accounts of 20th-century battles and campaigns.",200)},

            // Science 
            new Author{ NameEN="Carl Sagan", NameAR="كارل ساجان", BirthDate=new DateTime(1934,11,9), Bio=Truncate("American astronomer, cosmologist and science communicator, author of 'Cosmos' and other influential books.",200)},
            new Author{ NameEN="Stephen Hawking", NameAR="ستيفن هوكينغ", BirthDate=new DateTime(1942,1,8), Bio=Truncate("British theoretical physicist best known for his work on black holes and time, author of 'A Brief History of Time'.",200)},
            new Author{ NameEN="Richard Dawkins", NameAR="ريتشارد دوكنز", BirthDate=new DateTime(1941,3,26), Bio=Truncate("British evolutionary biologist, author of 'The Selfish Gene' among others.",200)},
            new Author{ NameEN="Rebecca Skloot", NameAR="ريبيكا سكولوت", BirthDate=new DateTime(1972,8,26), Bio=Truncate("American science writer best known for 'The Immortal Life of Henrietta Lacks'.",200)},
            new Author{ NameEN="Siddhartha Mukherjee", NameAR="سيدهارتا موكيرجي", BirthDate=new DateTime(1970,7,21), Bio=Truncate("Indian-American physician and author of 'The Gene: An Intimate History'.",200)},
            new Author{ NameEN="Rachel Carson", NameAR="راشيل كارسون", BirthDate=new DateTime(1907,5,27), Bio=Truncate("American marine biologist and conservationist whose book 'Silent Spring' advanced environmental movement.",200)},
            new Author{ NameEN="Thomas S. Kuhn", NameAR="توماس س. كون", BirthDate=new DateTime(1922,7,18), Bio=Truncate("American philosopher of science, known for theory of scientific revolutions.",200)},
            new Author{ NameEN="Daniel Kahneman", NameAR="دانييل كانيمان", BirthDate=new DateTime(1934,3,5), Bio=Truncate("Israeli-American psychologist and economist, author of 'Thinking, Fast and Slow'.",200)},
            new Author{ NameEN="Yuval Noah Harari", NameAR="يوفال نوح هراري", BirthDate=new DateTime(1976,2,24), Bio=Truncate("Israeli public intellectual, historian and bestselling author of 'Sapiens'.",200)},
            new Author{ NameEN="James Watson", NameAR="جيمس واتسون", BirthDate=new DateTime(1928,4,6), Bio=Truncate("American molecular biologist and co-discoverer of the structure of DNA, author of 'The Double Helix'.",200)},
        };



        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();
    }
    private static string Truncate(string s, int len) => s.Length <= len ? s : s.Substring(0, len);

}
