namespace FreeActions

open System

type ActionInstruction<'a> = 
    | Invoke of Action * (unit -> 'a)

type ActionProgram<'a> =
    | Free of ActionInstruction<ActionProgram<'a>>
    | Pure of 'a

module Actions =
    let private mapI f = function
    | Invoke (x, next) -> Invoke (x, next >> f)

    let rec private bind f = function
    | Free x -> x |> mapI (bind f) |> Free
    | Pure x -> f x
        
    let private invoke a = Free (Invoke (a, Pure))

    let abort = Pure ()

    let bindInvoke (a, p) = bind (fun () -> invoke a) p
    
    let rec interpret = function
    | Pure x -> x
    | Free (Invoke (x, next)) -> x.Invoke () |> next |> interpret
