import './App.scss';
import CarData from "./CarData/CarData";
import CarSelection from "./CarSelection/CarSelection";
import {Component} from "react";

export default class App extends Component {
    constructor() {
        super();

        this.selectCar = this.selectCar.bind(this)

        this.state = {
            data: {},
            ws: null
        };
    }

    selectCar(carId) {
        if(this.state.ws) {
            this.state.ws.close()
        }

        let ws = new WebSocket("wss://vroom.alnmrc.com/api/v1/ws/"+carId);

        ws.onmessage = (message) => {
            this.setState({data: message.Data});
            console.log(message)
        }

        this.setState({ws: ws})
    }

    render() {
        return (
            <div className="App">
                <CarSelection selectFunction={this.selectCar}/>
                <CarData/>
            </div>
        );
    }
}
