(async function () {
  fetch('.auth/me', {
    method: 'GET',
    headers: {
      'Content-type': 'application/json; charset=UTF-8',
    },
  })
    .then(function (response) {
      if (response.ok) {
        return response.json();
      }
      return Promise.reject(response);
    })
    .then(function (data) {
      let taskContainer = document.getElementById('task-container');
      let logInContainer = document.getElementById('log-in-container');
      let logOutContainer = document.getElementById('log-out-container');
      let loggedInAsUser = document.getElementById('logged-in-as-user');

      if (data.clientPrincipal != null) {
        taskContainer.classList.remove('d-none');
        logInContainer.classList.add('d-none');
        logOutContainer.classList.remove('d-none');
        loggedInAsUser.innerText = data.clientPrincipal.userDetails;

        fetchOwnerTasks(data.clientPrincipal.userDetails);
      } else {
        taskContainer.classList.add('d-none');
        logInContainer.classList.remove('d-none');
        logOutContainer.classList.add('d-none');
      }
      console.log(data);
    })
    .catch(function (error) {
      console.warn('Something went wrong.', error);
    });
})();

function fetchOwnerTasks(ownerName) {
  let url = 'api/fetchtasksapi';

  fetch(url, {
    method: 'POST',
    body: JSON.stringify({
      owner: ownerName,
    }),
    headers: {
      'Content-type': 'application/json; charset=UTF-8',
    },
  })
    .then(function (response) {
      if (response.ok) {
        return response.json();
      }
      return Promise.reject(response);
    })
    .then(function (data) {
      console.log('returned data :: ', data);
      printTasks(data);
    })
    .catch(function (error) {
      console.warn('Something went wrong.', error);
    });
}

function printTasks(tasks) {
  if (tasks.length > 0) {
    document.getElementById('task-list-container').classList.remove('d-none');
    document.getElementById('no-task-container').classList.add('d-none');

    for (let i = 0; i < tasks.length; i++) {
      const task = tasks[i];
      var table = document.getElementById('task-list-body');
      var row = table.insertRow(0);
      var cell1 = row.insertCell(0);
      var cell2 = row.insertCell(1);
      var cell3 = row.insertCell(2);
      cell1.innerHTML = task.name;
      cell2.innerHTML = task.startDate;
      cell3.innerHTML = task.minutesSpent;
    }
  } else {
    document.getElementById('task-list-container ').classList.add('d-none');
    document.getElementById('no-task-container').classList.remove('d-none');
  }
}

function submitTask() {
  let url = 'api/loggerapi';
  var taskForm = document.getElementById('task-form');
  taskForm.onsubmit = function (e) {
    e.preventDefault();
    // window.location.reload();
  };

  console.log(
    'user :: ',
    document.getElementById('logged-in-as-user').innerText
  );

  fetch(url, {
    method: 'POST',
    body: JSON.stringify({
      name: `${document.getElementById('task-name').value}`,
      owner: `${document.getElementById('logged-in-as-user').innerText}`,
    }),
    headers: {
      'Content-type': 'application/json; charset=UTF-8',
    },
  })
    .then(function (response) {
      if (response.ok) {
        console.log("response :: ", response);
        return response.json();
      }
      return Promise.reject(response);
    })
    .then(function (data) {
      console.log(data);
    })
    .catch(function (error) {
      console.warn('Something went wrong.', error);
    });
}
