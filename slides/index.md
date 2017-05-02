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
                    password)
    {
        //Validate
        //Persist
    }

}
```

---

``` C#
//...
CustomerService.CreateCustomer(
    "asdfgh-1234-1234",
    "aprooks",
    "aprooks@live.ru",
    "Prooks",
    "Alexander",
    "somePass",
    "79062190016");

```
---
[Introduce Parameter Object (c) Fowler](https://refactoring.com/catalog/introduceParameterObject.html)
```
[Serializable]
public class CreateCustomerDto
{
    public Result CreateCustomer(
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
        this.SurName = lastName;
        this.Phone = phone;
        this.Password  = password
    }

    public string ID {get;}
    public string Username {get;}
    //... the rest of boilerplate
    }

```
---
``` C#
//...
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
            result = Polly. service.Handle(request);
        }
        
        catch(Exception ex)
        {
            return Error.Exception(ex)
        }
        return Result.Handled(result);
    }
```




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
