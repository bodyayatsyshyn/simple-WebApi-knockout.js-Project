function Employee(item) {
    this.Id = item.id;
    this.Name = item.name;
    this.Surname = item.surname;
    this.DateOfBirth = new Date(item.dateOfBirth).toLocaleDateString('en-GB');
}


function EmployeeVW(mainVM, invoiceVM) {



    var self = this;
    self.Employees = ko.observableArray([]);
    self.UpdateTable = function () {
        $.getJSON("/api/employee",
            function (data) {
                var mappedData = $.map(data,
                    function (item) {
                        return new Employee(item);
                    });
                self.Employees(mappedData);
            }
        );
    }
    self.UpdateTable();


    function ObservableEmployee(emp) {

        var currentDate = (new Date()).toISOString().split('T')[0];
        this.Id = ko.observable(0);
        this.Name = ko.observable('');
        this.Surname = ko.observable('');
        this.DateOfBirth = ko.observable(currentDate);

        if (typeof emp !== 'undefined') {
            this.Id(emp.Id);
            this.Name(emp.Name);
            this.Surname(emp.Surname);
            var dateParts = emp.DateOfBirth.split("/");

            var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0] + 1);

            this.DateOfBirth(dateObject.toISOString().split('T')[0]);
        }
    }

    
    self.CurrentEmployee = ko.observable(new ObservableEmployee());

    self.TryDelete = function(employee, event) {
        self.CurrentEmployee(new ObservableEmployee(employee));
        $('#deleteEmployeeConfirmation').modal();
    }

    self.Delete = function() {
        $('#deleteEmployeeConfirmation').modal('hide');

        $.ajax({
            type: "DELETE",
            url: "/api/Employee/" + self.CurrentEmployee().Id(),
            success: function () {
                self.UpdateTable();
            },
            error: function () {
                alert("Failed");
            }
        });
    }

    self.TryUpdate = function (employee, event) {
        self.CurrentEmployee(new ObservableEmployee(employee));
        $('#editEmployeeModalForm').modal();
    }

    self.Update = function() {
        $('#editEmployeeModalForm').modal('hide');
            $.ajax({
                type: "PUT",
                url: "/api/Employee/" + self.CurrentEmployee().Id(),
                data: ko.toJSON(self.CurrentEmployee), 
                contentType: "application/json",
                success: function () {
                    self.UpdateTable();
                },
                error: function () {
                    alert("Failed");
                }
            });
    }

    self.resetCurrentEmployee = function() {
        self.CurrentEmployee(new ObservableEmployee());
    }

    self.Create = function () {
        $('#createEmployeeModalForm').modal('hide');
        $.ajax({
            type: "POST",
            url: "/api/Employee",
            data: ko.toJSON(self.CurrentEmployee()), 
            contentType: "application/json",
            success: function () {
                self.UpdateTable();
            },
            error: function () {
                alert("Failed");
            }
        });
    }
}

$(document).ready(function () {
    ko.applyBindings(new EmployeeVW());
});
