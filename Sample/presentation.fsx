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

