import http from 'k6/http';
import { check } from 'k6';

export const options = {
    scenarios: {
        load_test: {
            executor: 'ramping-vus',

            stages: [
                { duration: '20s', target: 100 },
                { duration: '20s', target: 200 },

            ],

            gracefulRampDown: '10s',
        },
    },

    insecureSkipTLSVerify: true,
};


export default function () {

    // Create a unique ID for each request
    const id = `${__VU}-${__ITER}`;


    // Send POST request
    const response = http.get(
        'http://localhost:5000/api/catalog/storefront/categories?sortBy=Name&sortDirection=Asc&pageNumber=1&pageSize=50',
        
		{
			timeout: '1600s'
		}
    );

    // Check response status
    const success = check(response, {
        'status is 2xx': (r) =>
            r.status >= 200 && r.status < 300
    });

    // Only print failed requests
    if (!success) {
        console.log(
            `Request failed | VU=${__VU} | ITER=${__ITER} | STATUS=${response.status}`
        );

   console.log(`Error: ${response.error}`);
    console.log(`Error code: ${response.error_code}`);
   
        console.log(`Response: ${response.body}`);
    }
}