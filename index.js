const source = new EventSource("http://localhost:5154/items")
const list = document.getElementById("ul-items")

source.addEventListener("normal", (e) => {
    const li = document.createElement("li")
    li.innerText = e.data
    li.className = "normal"
    list.appendChild(li)
})

source.addEventListener("special", (e) => {
    const li = document.createElement("li")
    li.innerText = e.data
    li.className = "special"
    list.appendChild(li)
})

