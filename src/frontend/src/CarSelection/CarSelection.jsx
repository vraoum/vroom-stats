import "./CarSelection.scss"
import {Component} from "react";
import {Button, Form, Modal} from "react-bootstrap";
import axios from "axios";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {faGear, faRotate} from "@fortawesome/free-solid-svg-icons";

export default class CarSelection extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showSettings: false,
            carTitle: this.props.car?.settings?.carTitle??'',
        }

        this.change = this.change.bind(this)
        this.closeModal = this.closeModal.bind(this)
        this.showSettings = this.showSettings.bind(this)
        this.handleInputChange = this.handleInputChange.bind(this)
        this.saveSettings = this.saveSettings.bind(this)
    }

    componentDidUpdate(prevProps, prevState, snapshot) {
        if(prevProps.car !== this.props.car) {
            this.setState({
                carTitle: this.props.car.settings.carTitle
            })
        }
    }

    componentDidMount() {
        this.props.fetchFunction()
    }

    change(event) {
        if(event.target.value) {
            this.props.selectFunction(event.target.value)
        }
    }

    closeModal() {
        this.setState({showSettings: false})
    }

    showSettings() {
        this.setState({showSettings: true})
    }

    handleInputChange(event) {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }

    saveSettings (event) {
        event.preventDefault()
        axios.put('https://vroom.alnmrc.com/api/v1/Cars/'+this.props.car.id,{
            settings: {
                carTitle: this.state.carTitle
            }
        }).then(_ => {this.closeModal(); this.props.fetchFunction()})
    }

    render() {
        let i = 0;
        let carsOptions = [<option key={i++}>Select a car</option>];

        this.props.cars.forEach(car => {
            carsOptions.push(<option key={i++} value={car.id}>{car?.settings?.carTitle??car.id}</option>)
        })
        let actions = [
            <Button key={i++} onClick={this.props.fetchFunction}><FontAwesomeIcon icon={faRotate}/></Button>
        ];
        if(this.props.car !== null) {
            actions.push(<Button key={i++} onClick={this.showSettings}><FontAwesomeIcon icon={faGear}/></Button>)
        }

        return(
            <div style={{display: "flex"}}>
                <Form.Select onChange={this.change} value={this.props.car ? this.props.car.id : undefined}>
                    {carsOptions}
                </Form.Select>
                {actions}

                <Modal show={this.state.showSettings} onHide={this.closeModal}>
                    <Form onSubmit={this.saveSettings}>
                        <Modal.Header closeButton>
                            <Modal.Title>Modal heading</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                                <Form.Group className="mb-3" controlId="carTitle">
                                    <Form.Label>Car title</Form.Label>
                                    <Form.Control
                                        placeholder="Short name of the car to retrieve it in the list"
                                        name={"carTitle"}
                                        value={this.state.carTitle}
                                        onChange={this.handleInputChange}
                                    />
                                </Form.Group>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={this.closeModal} type={"button"}>
                                Close
                            </Button>
                            <Button variant="primary" type={"submit"}>
                                Save Changes
                            </Button>
                        </Modal.Footer>
                    </Form>
                </Modal>
            </div>

        )
    }
}