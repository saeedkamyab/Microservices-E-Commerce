import http from 'k6/http';
import { check } from 'k6';

export const options = {
    scenarios: {
        load_test: {
            executor: 'ramping-vus',

            stages: [
                { duration: '20s', target: 5 }
               

            ],

            gracefulRampDown: '10s',
        },
    },

    insecureSkipTLSVerify: true,
};


export default function () {

    // Create a unique ID for each request
    const id = `${__VU}-${__ITER}`;

    // Request body
    const payload = JSON.stringify({
      
		Name:`LoadTest-Product-${id}`,
        ProductDescription:"No Des",
		CategoryId:"69e62140-e8c2-4e1a-94a3-81ceaa7415e6",
		Price:"1200",
		Specifications:[
			{
				attributeDefinitionId: "a3892215-88d2-4746-bdaf-0976168095ef",
				value: "Tesco"
			},
			{
				attributeDefinitionId: "f3311ed5-a619-4ce4-b86f-015eecdcdb98",
				value: "12"
			}
     
  ]
		
    });

    // Send POST request
    const response = http.post(
        'http://localhost:5227/api/backoffice/products',
        payload,
        {
            headers: {
                'Content-Type': 'application/json'
            }
        },
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