import "./SpeedGauge.scss"
import {Component} from "react";
import Gauge from "../../Components/Gauge/Gauge";

export default class SpeedGauge extends Component {
    render() {
        return (
            <div className="speedGauge">
                <Gauge
                    value={this.props.data.speed}
                    unit={"kmh"}
                    ringColor={"linear-gradient(90deg, rgba(47,226,111,1) 0%, rgba(252,176,69,1) 30%, rgba(253,29,29,1) 100%)"}
                    ringValue={this.props.data.rpm}
                    ringMax={this.props.car?.settings.maxRpmEnd??5500}
                    ringGraduateInterval={1000}
                    ringGraduateDisplayFunction={(val) => val/1000}
                    ringWarningStart={this.props.car?.settings.maxRpmStart??4500}
                    ringWarningEnd={this.props.car?.settings.maxRpmEnd??5500}
                    ringWarningColor={"#ff5a5a"}
                />
            </div>
        )
    }
}