using System;
using System.Collections.Generic;

// Класс исключений для количества автомобилей
public class KolichestvoAvtomobileyException : Exception
{
    public KolichestvoAvtomobileyException(string message) : base(message) { }
}

// Класс исключений для ФИО покупателя
public class FIOException : Exception
{
    public FIOException(string message) : base(message) { }
}

public class Avtosalon
{
    public string Nazvanie { get; set; }
}

public class Avtomobil
{
    public string Marka { get; set; }
    public int MaxPassengers { get; set; }
    public decimal Stoimost { get; set; }
    public int KolichestvoNaSklade { get; set; }
    public bool Naliechie { get; set; }
}

public class ZayavkaNaPokupku
{
    public string FioPokupatelya { get; set; }
    public string NomerTelefona { get; set; }
    public List<Avtomobil> Avtomobili { get; set; }

    public ZayavkaNaPokupku(string fio, string telefon)
    {
        if (string.IsNullOrEmpty(fio))
        {
            throw new FIOException("Фио покупателя не может быть пустым");
        }
        FioPokupatelya = fio;
        NomerTelefona = telefon;
        Avtomobili = new List<Avtomobil>();
    }

    public void AddAvtomobil(Avtomobil avtomobil)
    {
        Avtomobili.Add(avtomobil);
    }

    public void RemoveAvtomobil(Avtomobil avtomobil)
    {
        Avtomobili.Remove(avtomobil);
    }

    public virtual decimal RaschitatStoimostZakaza()
    {
        decimal stoimost = 0;
        foreach (var avtomobil in Avtomobili)
        {
            stoimost += avtomobil.Stoimost;
        }
        return stoimost;
    }
}

public class ZayavkaNaPriobretenieSoStenda : ZayavkaNaPokupku
{
    public ZayavkaNaPriobretenieSoStenda(string fio, string telefon) : base(fio, telefon) { }

    public override decimal RaschitatStoimostZakaza()
    {
        try
        {
            if (Avtomobili.Count == 0)
            {
                throw new KolichestvoAvtomobileyException("Количество автомобилей в заказе равно нулю");
            }
            return base.RaschitatStoimostZakaza();
        }
        catch (KolichestvoAvtomobileyException e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Неизвестная ошибка: " + e.Message);
            return 0;
        }
    }
}

public class ZayavkaNaOtlozhennuyuPostavku : ZayavkaNaPokupku
{
    public decimal ProcentSkidki { get; set; }

    public ZayavkaNaOtlozhennuyuPostavku(string fio, string telefon, decimal procentSkidki) : base(fio, telefon)
    {
        ProcentSkidki = procentSkidki;
    }

    public override decimal RaschitatStoimostZakaza()
    {
        try
        {
            if (Avtomobili.Count == 0)
            {
                throw new KolichestvoAvtomobileyException("Количество автомобилей в заказе равно нулю");
            }
            decimal stoimost = base.RaschitatStoimostZakaza();
            return stoimost - (stoimost * ProcentSkidki / 100);
        }
        catch (KolichestvoAvtomobileyException e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Неизвестная ошибка: " + e.Message);
            return 0;
        }
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            ZayavkaNaPokupku zayavka1 = new ZayavkaNaPriobretenieSoStenda("Иван Иванов", "123456789");
            Console.WriteLine("Стоимость заказа: " + zayavka1.RaschitatStoimostZakaza());

            ZayavkaNaPokupku zayavka2 = new ZayavkaNaOtlozhennuyuPostavku("Пётр питров", "987654321", 10);
            zayavka2.AddAvtomobil(new Avtomobil { Marka = "BMW", Stoimost = 50000 });
            Console.WriteLine("Стоимость заказа: " + zayavka2.RaschitatStoimostZakaza());

            // Проверка исключения FIOException
            ZayavkaNaPokupku zayavka3 = new ZayavkaNaPriobretenieSoStenda("", "111222333");
        }
        catch (FIOException e)
        {
            Console.WriteLine("Невозможно создание заказа, не указано фио покупателя: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Общая ошибка: " + e.Message);
        }
    }
}
