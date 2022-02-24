import "./Gauge.scss"
import {Component} from "react";

export default class Gauge extends Component {
    render() {
        let graduations = [];
        if(this.props.ringGraduateInterval) {
            let ringGraduateDisplayFunction = (i) => i;
            if(this.props.ringGraduateDisplayFunction) {
                ringGraduateDisplayFunction = this.props.ringGraduateDisplayFunction;
            }
            for(let i = 1 ; i <= this.props.ringMax / this.props.ringGraduateInterval ; i++) {
                let rotation = i * this.props.ringGraduateInterval * 180 / this.props.ringMax;
                graduations.push(
                    <div className="graduation" key={i} style={{rotate: rotation + "deg"}}>
                        <div className="graduation-value" style={{rotate: -rotation + "deg"}}>
                            {ringGraduateDisplayFunction(i * this.props.ringGraduateInterval)}
                        </div>
                    </div>
                )
            }
        }
        return(
            <div className="gauges">
                <div className="innerGauge">
                    <div
                        className="gauge"
                        style={{
                            background: this.props.ringColor
                        }}>
                        <div className="percentage" style={{
                            rotate: this.props.ringValue * 180 / this.props.ringMax + "deg",
                            backgroundColor: this.props.ringOtherColor ?? '#fff'
                        }}/>
                        <div className="mask"/>
                        <span className="value">{(this.props.value ?? '') + (this.props.unit ?? '')}</span>
                    </div>
                </div>
                <div className="outerGauge">
                    <div className="gauge" style={{
                        backgroundColor: this.props.ringWarningColor ?? '#fff'
                    }}>
                        <div className="percentage" style={{
                            rotate: (this.props.ringWarningEnd??0) * 180 / this.props.ringMax + "deg",
                            backgroundColor: '#fff'
                        }}/>
                        <div className="percentage" style={{
                            rotate: (180 + (this.props.ringWarningStart??0) * 180 / this.props.ringMax) + "deg",
                            backgroundColor: '#fff'
                        }}/>
                        <div className="mask"/>

                    </div>
                    {graduations}
                </div>
            </div>
        )
    }
}