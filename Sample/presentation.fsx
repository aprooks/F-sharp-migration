module Dtos

type CreateCustomer = {
    id: string
    username: string
    email: string
    phone: string
    name: string
    lastName: string
    password: string
}

let dto = {
    id= "test"
    username= "aprooks"
    email= "aprooks@live.ru"
    phone= "79062190016"
    name= "Alexander"
    lastName= "Prooks"
    password="secret"
}

let a = dto
a = dto //true

let b = {a with id="Test2"}
b=a //false


//DDD


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

let id = Id "test"
let Username = Username "test"

//id = Username //compile error

type Gender = 
| Male
| Female
| Other of string

let male = Male
let female = Other "111"

let toString = function 
                | Male -> "male"
                | Female -> "female"
                | Other s -> s

let fromString = function
                  | "male" -> Male
                  | "female" -> Female
                  | other -> Other other



module Test =                    
    // static int sum(int a, int b) = { return a+b}
    
    // int -> int -> int
    let sum (a:int) (b:int) = a+b     

    // static int sum(decimal a, decimal b) = { return a+b}
    // etc 

    // 'a -> 'a -> 'a
    let inline sum1 a b = a + b   //WAT??

    let d = 10m + 10m
    let c = "test" + "passed"
    //let d = 100 + "test" //error

module Currying = 
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
    
    