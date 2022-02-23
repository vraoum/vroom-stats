import "./CarSelection.scss"
import {Component} from "react";
import {Form} from "react-bootstrap";
import axios from "axios";

export default class CarSelection extends Component {
    constructor(props) {
        super(props);

        this.change = this.change.bind(this)

        this.state = {
            cars: []
        }
    }

    componentDidMount() {
        this.fetchCars()
    }

    fetchCars() {
        axios.get('https://vroom.alnmrc.com/api/v1/Cars').then(
            resp => this.setState({cars: resp.data})
        )
    }

    change(event) {
        this.props.selectFunction(event.target.value)
    }

    render() {
        let carsOptions = [];
        let i = 0;
        this.state.cars.forEach(car => {
            carsOptions.push(<option key={i++} value={car.id}>{car.displayName}</option>)
        })
        return(
            <div>
                <Form.Select onChange={this.change}>
                    {carsOptions}
                </Form.Select>
            </div>
        )
    }
}