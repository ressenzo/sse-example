const source = new EventSource("http://localhost:5154/items")
const list = document.getElementById("ul-items")

source.addEventListener("normal", (e) => {
    const li = document.createElement("li")
    li.innerText = e.data
    li.className = "normal"
    list.appendChild(li)
})

source.addEventListener("especial", (e) => {
    const li = document.createElement("li")
    li.innerText = e.data
    li.className = "especial"
    list.appendChild(li)
})

