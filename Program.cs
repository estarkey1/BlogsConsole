using NLog;

class Program
{
    static void Main(string[] args)
    {
        string path = Directory.GetCurrentDirectory() + "\\nlog.config";
        var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
        logger.Info("Program started");

        try
        {
            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Display all blogs");
                Console.WriteLine("2. Add Blog");
                Console.WriteLine("3. Create Post");
                Console.WriteLine("4. Display Posts");
                Console.WriteLine("5. Exit");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    DisplayAllBlogs(logger);
                }
                else if (choice == "2")
                {
                    AddBlog(logger);
                }
                else if (choice == "3")
                {
                    CreatePost(logger);
                }
                else if (choice == "4")
                {
                    DisplayPosts(logger);
                }
                else if (choice == "5")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }

        logger.Info("Program ended");
    }

    static void DisplayAllBlogs(Logger logger)
    {
        using (var db = new BloggingContext())
        {
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }
    }

    static void AddBlog(Logger logger)
    {
        Console.Write("Enter a name for the new Blog: ");
        var name = Console.ReadLine();

        var blog = new Blog { Name = name };

        using (var db = new BloggingContext())
        {
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);
        }
    }

    static void CreatePost(Logger logger)
    {
        using (var db = new BloggingContext())
        {
            DisplayAllBlogs(logger);
            Console.Write("Enter the ID of the Blog you are posting to: ");
            if (int.TryParse(Console.ReadLine(), out int blogId))
            {
                var blog = db.Blogs.Find(blogId);
                if (blog != null)
                {
                    Console.Write("Enter post title: ");
                    var title = Console.ReadLine();
                    Console.Write("Enter post content: ");
                    var content = Console.ReadLine();

                    var post = new Post { Title = title, Content = content, BlogId = blogId };
                    db.Posts.Add(post);
                    db.SaveChanges();
                    logger.Info("Post added to Blog - {blogName}: {postTitle}", blog.Name, title);
                }
                else
                {
                    Console.WriteLine("Invalid Blog ID. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid ID.");
            }
        }
    }

    static void DisplayPosts(Logger logger)
    {
        using (var db = new BloggingContext())
        {
            DisplayAllBlogs(logger);
            Console.Write("Enter the ID of the Blog whose posts you want to view: ");
            if (int.TryParse(Console.ReadLine(), out int blogId))
            {
                var blog = db.Blogs.Find(blogId);
                if (blog != null)
                {
                    var posts = db.Posts.Where(p => p.BlogId == blogId).ToList();
                    Console.WriteLine($"Number of posts for {blog.Name}: {posts.Count}");
                    foreach (var post in posts)
                    {
                        Console.WriteLine($"Blog Name: {blog.Name}");
                        Console.WriteLine($"Post Title: {post.Title}");
                        Console.WriteLine($"Post Content: {post.Content}");
                        Console.WriteLine("----------------------");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Blog ID. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid ID.");
            }
        }
    }
}