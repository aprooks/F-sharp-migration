- title : F#: Migration guid
- description : Why F# is awesome and how to start using it right now
- author : Alexander Prooks 
- theme : night
- transition : default

***

## F# |> LV

<br />
<br />

### Simple ways of daily usage

<br />
<br />
Alexander Prooks - [@aprooks](http://www.twitter.com/aprooks)

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
                    password)
    {
        this.Id = id;
        this.Username = username;
        this.Name = name;
        this.Surname = lastName;
        this.Phone = phone;
        this.Password  = password
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
                            password:"helloWorld"
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
    //Consstructor
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

## let (|>) x f = f x

---

## Reading signatures

    // string -> string
    let append (tail:string) string = "Hello " + tail
    
    // infered types:
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
    
    let consoleLogger output = printfn "%s: %s" (System.DateTime.Now.ToString("HH:mm:ss.f")) output
    
    let result = sample consoleLogger 
                        (
                            fun () -> 
                                System.Threading.Thread.Sleep(500)
                                42
                        )

---

## Currying

    //string -> string
    let prepend x = concat x " world"


    let dbExecute connection command = 




---

## 'a -> 'a -> 'a

    // int -> int -> int
    let sum (a:int) (b:int) = a+b     

    // static int sum(decimal a, decimal b) = { return a+b}
    // etc 

    // 'a -> 'b -> 'c
    //           when ( ^a or  ^b) : (static member ( + ) :  ^a *  ^b ->  ^c)
    let inline sum1 a b = a + b   //WAT??

    let d = 10m + 10m
    let c = "test" + "passed"
    let d = 100 + "test" //error

***


### "Native" UI

 <img src="images/meter.png" style="background: transparent; border-style: none;"  width=300 />

---

### Tooling

<img src="images/hotloading.gif" style="background: transparent; border-style: none;"  />

*** 

### Model - View - Update

#### "Elm - Architecture"

 <img src="images/Elm.png" style="background: white;" width=700 />


 <small>http://danielbachler.de/2016/02/11/berlinjs-talk-about-elm.html</small>


--- 

### Model - View - Update

    // MODEL

    type Model = int

    type Msg =
    | Increment
    | Decrement

    let init() : Model = 0

---

### Model - View - Update

    // VIEW

    let view model dispatch =
        div []
            [ button [ OnClick (fun _ -> dispatch Decrement) ] [ str "-" ]
              div [] [ str (model.ToString()) ]
              button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ] ]

---

### Model - View - Update

    // UPDATE

    let update (msg:Msg) (model:Model) =
        match msg with
        | Increment -> model + 1
        | Decrement -> model - 1

---

### Model - View - Update

    // wiring things up

    Program.mkSimple init update view
    |> Program.withConsoleTrace
    |> Program.withReact "elmish-app"
    |> Program.run

---

### Model - View - Update

# Demo

***

### Sub-Components

    // MODEL

    type Model = {
        Counters : Counter.Model list
    }

    type Msg = 
    | Insert
    | Remove
    | Modify of int * Counter.Msg

    let init() : Model =
        { Counters = [] }

---

### Sub-Components

    // VIEW

    let view model dispatch =
        let counterDispatch i msg = dispatch (Modify (i, msg))

        let counters =
            model.Counters
            |> List.mapi (fun i c -> Counter.view c (counterDispatch i)) 
        
        div [] [ 
            yield button [ OnClick (fun _ -> dispatch Remove) ] [  str "Remove" ]
            yield button [ OnClick (fun _ -> dispatch Insert) ] [ str "Add" ] 
            yield! counters ]

---

### Sub-Components

    // UPDATE

    let update (msg:Msg) (model:Model) =
        match msg with
        | Insert ->
            { Counters = Counter.init() :: model.Counters }
        | Remove ->
            { Counters = 
                match model.Counters with
                | [] -> []
                | x :: rest -> rest }
        | Modify (id, counterMsg) ->
            { Counters =
                model.Counters
                |> List.mapi (fun i counterModel -> 
                    if i = id then
                        Counter.update counterMsg counterModel
                    else
                        counterModel) }

---

### Sub-Components

# Demo

***

### React

* Facebook library for UI 
* <code>state => view</code>
* Virtual DOM

---

### Virtual DOM - Initial

<br />
<br />


 <img src="images/onchange_vdom_initial.svg" style="background: white;" />

<br />
<br />

 <small>http://teropa.info/blog/2015/03/02/change-and-its-detection-in-javascript-frameworks.html</small>

---

### Virtual DOM - Change

<br />
<br />


 <img src="images/onchange_vdom_change.svg" style="background: white;" />

<br />
<br />

 <small>http://teropa.info/blog/2015/03/02/change-and-its-detection-in-javascript-frameworks.html</small>

---

### Virtual DOM - Reuse

<br />
<br />


 <img src="images/onchange_immutable.svg" style="background: white;" />

<br />
<br />

 <small>http://teropa.info/blog/2015/03/02/change-and-its-detection-in-javascript-frameworks.html</small>


*** 

### ReactNative

 <img src="images/ReactNative.png" style="background: white;" />


 <small>http://timbuckley.github.io/react-native-presentation</small>

***

### Show me the code

*** 

### TakeAways

* Learn all the FP you can!
* Simple modular design

*** 

### Thank you!

* https://github.com/fable-compiler/fable-elmish
* https://ionide.io
* https://facebook.github.io/react-native/
