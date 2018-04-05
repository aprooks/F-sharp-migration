- title : Migrate to F#
- description : Why F# is awesome and how to start using it right now
- author : Alexander Prooks 
- theme : night
- transition : default

***

## F# |> LV

<br />
<br />

<br />
Alexander Prooks - [@aprooks](http://www.twitter.com/aprooks)


***


<br/>
<br/>
# apaleo 
<img src="images/leo.png" style="background: transparent; border-style: none;"  />

***

### Type system intro

#### non sucking C# handling pipe

``` C#
public class CustomerService
{
    public static Result CreateCustomer(
                    string id, 
                    string username, 
                    string email, 
                    string name,
                    string lastName, 
                    string phone, 
                    string password)
    {
        //Validate
        //Persist
    }

}
```

---
    [lang=csharp] 
    //...
    CustomerService.CreateCustomer(
        "asdfgh-1234-1234",
        "aprooks",
        "aprooks@live.ru",
        "Prooks",
        "Alexander",
        "somePass",
        "79062190016");
    
---
[Introduce Parameter Object (c) Fowler](https://refactoring.com/catalog/introduceParameterObject.html)
```
[Serializable]
public class CreateCustomerDto
{
    public CreateCustomer(
                    string id, 
                    string username, 
                    string email, 
                    string name,
                    string lastName, 
                    string phone, 
                    string password)
    {
        this.Id = id;
        this.Username = username;
        this.Name = name;
        this.Surname = lastName;
        this.Phone = phone;
        this.Password  = password;
    }
    public string ID {get;}
    public string Username {get;}
    //etc..
}
```

---

``` C#
var result = CustomerService.Handle( //method overloading
                  new CreateCustomerDto(  
                            id: "Id",
                            username: "Aprooks",
                            email: "aprooks@live.ru",
                            phone: "79062190016"
                            name: "Alexander",
                            lastName: "Prooks",
                            password: "helloWorld"
                ));
```

---

``` C#
//some wrapper
    public Result Handle<T>(T request){
        Log.Debug("Handled {request}",request);
        //https://github.com/JeremySkinner/FluentValidation/wiki
        var validator = ValidationFactory.GetValidator<T>();
        if(validator!=null){
            var validationResult = validator.Validate(request);
            if(!result.IsValid)
                return validationResult.ToError();
        }        
        if(Deduplicator.IsDuplicate(request))
            return Errors.DuplicateRequest;
        object result;
        try{
            result = Polly.Handle<TimeoutException>()
                          .Retry(5)
                          .Execute(()=> service.Handle(request));
        }
        catch(Exception ex)
        {
            return Error.Exception(ex)
        }
        return Result.Handled(result);
    }
```

***

## F# version

### Record types

    type CreateCustomer = {
        id: string
        username: string
        email: string
        phone: string
        name: string
        lastName: string
        password: string
    }

---

### Generated .net code

``` C#
[Serializable]
public sealed class CreateCustomer {
    IEquatable<CreateCustomer>,
    IStructuralEquatable,
    IComparable<CreateCustomer>,
    IComparable,
    IStructuralComparable
 
    //props
    //Constructor
    //Interfaces implementations
}
```

[Full comparison](https://fsharpforfunandprofit.com/posts/fsharp-decompiled/)

---
### Using records

    let dto = {
        id= "test"
        username= "aprooks"
        email= "aprooks@live.ru"
        phone= "79062190016"
        name= "Alexander"
        lastName= "Prooks"
        password= "secret"
    }

---

### Syntax sugar

    let copy = {
        id= "test"
        username= "aprooks"
        email= "aprooks@live.ru"
        phone= "79062190016"
        name= "Alexander"
        lastName= "Prooks"
        password= "secret"
    }
    copy = dto //values comparison
    //true
    
    let b = {a with id="Test2"}
    //!!!!

    b = a //false
---

## Self documented code

### Aliases

    type Id = string
    type Email = string
    type Username = string
    
    type CreateCustomer2 = {
        id: Id
        username: Username
        email: Email
        phone: string
        name: string
        lastName: string
        password: string
    }
    
---

### Enforced single-case types

    type Id = Id of string
    type Email = Email of string
    type Username = Username of string
    
    type Customer = {
        id: Id
        username: Username
        email: Email
        phone: string
        name: string
        lastName: string
        password: string
    }

---

### Compile time validation!

    let id = Id "test"
    let Username = Username "test"
    
    //id = Username //compile error
    
---

### Multiple case type (DU)

    type Gender = 
    | Male
    | Female
    | Other of string
    
    type CustomerWithGender = {
        // ... 
        Gender: Gender
    }
    
    let a = Male
    let b = Other "111"

---

### Pattern matching

    let toString gender= 
        match gender with 
        | Male -> "male"
        | Female -> "female"
        | Other s -> s

    //same as:
    let toString =
        function 
        | Male -> "male"
        | Female -> "female"
        | Other s -> s

---

### other way round

    let fromString = function
        | "male" -> Male
        | "female" -> Female
        | other -> Other other


---

### DDD

    type Percent = Percent of decimal
    type Amount = Amount of decimal
    type NumberOfNights = NumberOfNights of uint

    type Discount = 
    | ``Monetary per night`` of Amount
    | ``Percent per night`` of Percent
    | ``Monetary per stay`` of Amount
    | ``Monetary for extra guest per night`` of uint * Amount
    | ``Percent for extra stay`` of  NumberOfNights * Percentage

    // C#: public class MonetaryPerNight: IDiscount blah blah

---

### Types conclusion

* No boilerplate
* Readability
* Type safety for free

***

# Functions!

---

## Reading signatures

    // string -> string
    let append (tail:string) string = "Hello " + tail
    
    // inferred types:
    let append tail = "Hello " + tail
    
    // append 10 //compile error
    append "world" //"Hello world"
    

    // string -> string -> string
    let concat a b = a + b

    // unit -> int
    let answer() = 42

    // string -> unit
    let devnull _ = ignore() 

---

## Function as params

    // (string -> unit) -> (unit->'a)
    let sample logger f = 
        logger "started"
        let res = f()
        logger "ended"
        res
    
    let consoleLogger output =
        printfn "%s: %s" (System.DateTime.Now.ToString("HH:mm:ss.f")) output
    
    let result = sample consoleLogger 
                        (
                            fun () -> 
                                System.Threading.Thread.Sleep(500)
                                42
                        )

---

## 'a -> 'a -> 'a

    // int -> int -> int
    let sumInts (a:int) (b:int) = a+b     

    // static int sum(decimal a, decimal b) = { return a+b}
    // etc 

    // 'a -> 'b -> 'c
    //           when ( ^a or  ^b) : (static member ( + ) :  ^a *  ^b ->  ^c)
    let inline sum a b = a + b   //WAT??

    let d = 10m + 10m
    let c = "test" + "passed"
    let d = 100 + "test" //error

***
# let (|>) x f = f x

---

## Currying

    // string -> string -> string
    let concat x y = string.Concat(x,y)

    // <=>

    // string -> (string -> string)

    // string -> string
    let greet = concat "Hello"
    // <=>
    let greetVerbose w = concat "Hello" w

---

## DI F# way


    module Persistence = 

        let saveToDb connString (id,obj) = 
            // blah blah
            Success 

    module CompositionRoot =
        
        let connectionString = loadFromConfig("database")
        let persist = saveToDb connectionString

    let result = persist ("123",customer)
    
---

## Data pipe [0]

    let evenOnly = Seq.filter (fun n -> n%2=0) {0..1000}
    let doubled = Seq.map ((*) 2) evenOnly
    let stringified = Seq.map (fun d-> d.ToString()) doubled
    let greeted = Seq.map greet stringified

    // ["Hello 0","Hello 4", ...]
---

## Data pipes [1]

    let inline (|>) f x = x f   
    
    let evenF = (|>) ( {0..1000} ) ( Seq.filter (fun n -> n%2=0) )
    
    let evenInfix = {0..1000} |> ( Seq.filter (fun n -> n%2=0) )

---

## Piped data!

    {0..1000}
    |> Seq.filter (fun n -> n%2=0) //numbers
    |> Seq.map ((*) 2) //evenOnly
    |> Seq.map (fun d-> d.ToString()) //doubled
    |> Seq.map greet //stringified

---

## Real world like

    let handlingWrapper myHandler request = 
        request
        |> Log "Handling {request}"
        |> Validator.EnsureIsValid
        |> Deduplicator.EnsureNotDuplicate
        |> Throttle (Times 5) myHandler
        |> Log "Handling finished with {result}"

***

# How to migrate

* Utilities [Paket, Fake]
* Contracts 
* Helpers 
* Tests     [FsCheck, Expecto]
* Code as client 

***

# Marketing

* 2-20 times less code
* Better reuse
* Safer code => less bugs
* Human readable code => faster feedback

---

### F# in UI

<img src="images/hotloading.gif" style="background: transparent; border-style: none;"  />

---

## F#/OCaml ecosystem

* https://facebook.github.io/reason/
* https://github.com/alpaca-lang/alpaca
* http://elm-lang.org/
* http://fable.io/
* https://ionide.io

---


***

# Q?
