import './App.scss';
import CarData from "./CarData/CarData";
import CarSelection from "./CarSelection/CarSelection";
import {Component} from "react";
import axios from "axios";
import {Col, Row} from "react-bootstrap";
import CommandHistory from "./CommandHistory";

export default class App extends Component {
    constructor() {
        super();

        this.selectCar = this.selectCar.bind(this)
        this.fetchCars = this.fetchCars.bind(this)

        this.state = {
            data: {},
            car: null,
            cars: [],
            ws: null
        };
    }

    fetchCars() {
        axios.get('https://vroom.alnmrc.com/api/v1/Cars').then(
            resp => {
                this.setState({cars: resp.data, car: resp.data.filter(car => car.id === this.state.car?.id)[0]??null})
            }
        )
    }

    selectCar(carId) {
        if(this.state.ws) {
            this.state.ws.close()
            this.setState({data: {}, ws: null})
        }

        this.setState({car: this.state.cars.filter(car => car.id === carId)[0] ?? null})

        let ws = new WebSocket("wss://vroom.alnmrc.com/api/v1/ws/"+carId);

        ws.onmessage = (message) => {
            let messageData = JSON.parse(message.data)
            this.setState({data: messageData.Data});
        }

        this.setState({ws: ws})
    }

    render() {
        return (
            <div className="App">
                <Row>
                    <Col>
                        <CarSelection car={this.state.car} cars={this.state.cars} fetchFunction={this.fetchCars} selectFunction={this.selectCar}/>
                        <CarData data={this.state.data} car={this.state.car} />
                    </Col>
                    <Col>
                        <CommandHistory data={this.state.data} />
                    </Col>
                </Row>
            </div>
        );
    }
}
