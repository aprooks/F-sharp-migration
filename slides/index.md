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

<br>

https://apaleo.com/


***

### Defining a problem

---

#### What can go wrong?

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

### Uniform interface


``` C#
var result = CustomerService.Handle(
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
    // middleware
    public Result Handle<TService, T>(TService service, T request){
        Log.Debug("Handled {request}",request);

        var validator = ValidationFactory.GetValidator<T>();
        if(validator!=null){
            var validationResult = validator.Validate(request);
            if(!result.IsValid)
                return validationResult.ToError();
        }
        object result;
        try{
            result = Polly.Handle<TimeoutException>()
                          .Retry(5)
                          .Execute(()=> service.Handle(request));
        }
        catch(Exception ex)
        {
            return ex.ToError()
        }
        return Result.Handled(result);
    }
```

***

## Functional = data + (pure) functions

<br>

### F# = types + functions + imperative fallback

***

### Records types

* Flat data
* All fields are required => "AND type"
* Immutable by default (like everything else)

---

### Definition

``` F#
type CreateCustomer = {
    Id: string
    Username: string
    Email: string
    Phone: string
    Name: string
    LastName: string
    Password: string
}
```
---

### Generated C# code

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
### Init with data

``` F#
let dto = {
    Id= "test"
    Username= "aprooks"
    Email= "aprooks@live.ru"
    Phone= "79062190016"
    Name= "Alexander"
    LastName= "Prooks"
    Password= "secret"
}
```
---

### Syntax sugar

``` F#
let copyPasted = {
    Id= "test"
    Username= "aprooks"
    Email= "aprooks@live.ru"
    Phone= "79062190016"
    Name= "Alexander"
    LastName= "Prooks"
    Password= "secret"
}
copyPasted = dto //true

let b = {a with Id="Test2"} //copy

b = a //false

```

***

### Aliases aka document your types

    // I'm prototyping and not sure what it will be
    type Id = NotImplementedException 
    type Email = string
    type Username = string

    type CreateCustomer2 = {
        Id: Id
        Username: Username
        Email: Email
        Phone: string
        Name: string
        LastName: string
        Password: string
    }

***

### Discriminated union

* Pick only one of: "OR" type

---
    // Choose strictly one
    type ``Enum on steroids`` =
        | ``I am a valid case without data``
        | SomethingElse
        | ``I have data`` of Data
        | ``I am recursion`` of ``Enum on steroids``

---

### Single-case aka data wrapper

    type Id = 
        | Id of string
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

### Compile time validation

    let id = Id "test"
    let username = Username "test"
    
    //id = username //compile error

---

### Multiple case type (DU)

    // I wish it never happened  
    type SystemError =
        | DatabaseTimeout
        | Unauthorised
    
    // service module
    type CustomerServiceError = 
        | UserAlreadyExists

    // Composition root level
    type ApplicationErrors = 
        | System of SystemError
        | CustomerService of CustomerServiceError
        | OtherService of OtherServiceError
---

### Pattern matching to handle them all

    let toErrorMessage error = 
        match error with 
        | System err -> 
            match err with 
            | DatabaseTimeout err ->
                (HttpStatus.InternalServerError, "Ooops :(")
            | Unauthorised err ->
                (HttpStatus.Unauthorised, "Go away") 
        | CustomerService of err -> 
            match err with 
            | UserAlreadyExists -> 
                (HttpStatus.Conflict, "You are already registered")
        | OtherService of err -> 
            OtherService.ToErrorMessage err

---

### DU Patterns

---

### Option: Empty, but not null


    type Option<`a> = 
        | Some of `a
        | None

    type User = {
        Id : UserId
        Address : Address option
    }

    let OnUserRegistered user = 
        /// blabla
        match user.Address with 
        | Some addr -> sendPostcard addr
        | None -> ignore()

---
### Result: Done or error?

    type Result<'TSuccess,'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure

    let registerUser (load, save) user = 
        let dbUser = load user.Id
        match dbUser with 
        | None ->
            save user 
            Success(user.Id)
        | Some _ ->
            Error(UserService.AlreadyRegistered)

[Railway oriented programming](https://swlaschin.gitbooks.io/fsharpforfunandprofit/content/posts/recipe-part2.html)

---

### Data everyone else can trust

``` F#
type Id = 
    private Id of string

module Id = 
    let create (input : string) = 
        if (input.Length > 10)
            // Imperative style:
            failwith new ArgumentException()
        else
            Id ( input.ToUpperInvariant() )

type UserId = UserId of Id
```

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
    | ``Percent for extra stay`` of  NumberOfNights * Percent

    // C#: public class MonetaryPerNight: IDiscount 
    // blah blah
---

### Domain modelling made functional

<img src="images/Domain-modelling.jpg" style="background: transparent; border-style: none;"  />

<br>

[Where to buy](https://fsharpforfunandprofit.com/books/)

---

### Types conclusion

<br>

* No boilerplate
* Readability
* Type safety for free
* Design with types
* Unit test only interactions (functions)


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
    
    let result = sample consoleLogger (
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
        let save = saveToDb connectionString

    let result = save ("123",customer)
    
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
* Tests  [FsCheck, Expecto]
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
* https://ionide.io/

---

***

# Q?
