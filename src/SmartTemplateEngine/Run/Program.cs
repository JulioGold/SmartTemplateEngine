using SmartTemplateEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            string orderTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "assets", "orderTemplate.html");
            string orderTemplateItemsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "assets", "orderTemplateItems.html");
            string orderTemplate = File.ReadAllText(orderTemplatePath);
            string orderTemplateItems = File.ReadAllText(orderTemplateItemsPath);
            List<Order> orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    Number = 123,
                    Date = DateTime.Now,
                    Customer = new Person
                    {
                        Id = 1,
                        Name = "Julio",
                        Email = "julio@julio.com.br",
                        Phone = "999999999"
                    },
                    Total = 210,
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 1,
                            Description = "Item 1",
                            Value = 100
                        },
                        new OrderItem
                        {
                            Id = 2,
                            Description = "Item 2",
                            Value = 80
                        },
                        new OrderItem
                        {
                            Id = 3,
                            Description = "Item 3",
                            Value = 30
                        }
                    }
                },
                new Order
                {
                    Id = 2,
                    Number = 132,
                    Date = DateTime.Now,
                    Customer = new Person
                    {
                        Id = 2,
                        Name = "Cesar",
                        Email = "cesar@julio.com.br",
                        Phone = "111111111"
                    },
                    Total = 321,
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 4,
                            Description = "Item 1",
                            Value = 70
                        },
                        new OrderItem
                        {
                            Id = 5,
                            Description = "Item 2",
                            Value = 80
                        },
                        new OrderItem
                        {
                            Id = 6,
                            Description = "Item 3",
                            Value = 120
                        }
                        ,
                        new OrderItem
                        {
                            Id = 7,
                            Description = "Item 4",
                            Value = 20
                        }
                        ,
                        new OrderItem
                        {
                            Id = 8,
                            Description = "Item 5",
                            Value = 30
                        }
                        ,
                        new OrderItem
                        {
                            Id = 9,
                            Description = "Item 6",
                            Value = 1
                        }
                    }
                }
            };
            Dictionary<string, string> tagAndTemplate = new Dictionary<string, string>
            {
                {"{{Items}}", orderTemplateItems }
            };
            List<string> ordersResult = orders
                .Select(order => SmartTemplateEngineBuilder.ProcessTemplate(orderTemplate, order, tagAndTemplate))
                .ToList();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ItemsAttribute : Attribute
    {
        private readonly string _tag;

        public ItemsAttribute(string tag)
        {
            _tag = tag;
        }
    }

    public sealed class Order
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public DateTime Date { get; set; }

        public Person Customer { get; set; }

        public decimal Total { get; set; }

        public List<OrderItem> Items { get; set; }
    }

    public sealed class OrderItem
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }
    }

    public sealed class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
