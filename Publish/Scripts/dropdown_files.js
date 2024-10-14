document
    .querySelector(".c-filter__toggle")
    .addEventListener("click", function () {
        this.classList.toggle("c-filter__toggle--active");
        document
            .querySelector(".c-filter__ul")
            .classList.toggle("c-filter__ul--active");
    });