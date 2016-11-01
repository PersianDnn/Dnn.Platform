import React, { Component, PropTypes } from "react";
import { connect } from "react-redux";
import "./style.less";
import SingleLineInputWithError from "dnn-single-line-input-with-error";
import Grid from "dnn-grid-system";
import Label from "dnn-label";
import Button from "dnn-button";
import Switch from "dnn-switch";
import Select from "dnn-select";
import DropdownWithError from "dnn-dropdown-with-error";
import MultiLineInput from "dnn-multi-line-input";
import InputGroup from "dnn-input-group";
import Input from "dnn-single-line-input";
import Dropdown from "dnn-dropdown";
import {
    siteSettings as SiteSettingsActions
} from "../../../actions";
import util from "../../../utils";
import resx from "../../../resources";

class SiteAliasEditor extends Component {
    constructor() {
        super();

        this.state = {
            aliasDetail: {
                HTTPAlias: "",
                BrowserType: "Normal",
                Skin: "",
                CultureCode: "",
                IsPrimary: false
            },
            error: {
                alias: true
            },
            triedToSubmit: false
        };
    }

    componentWillMount() {
        const {props} = this;
        if (props.aliasId) {
            props.dispatch(SiteSettingsActions.getSiteAlias(props.aliasId));
        }
    }

    componentWillReceiveProps(props) {
        let {state} = this;

        if (!props.aliasDetail) {
            return;
        }

        if (props.aliasDetail["HTTPAlias"] === undefined || props.aliasDetail["HTTPAlias"] === "") {
            state.error["alias"] = true;
        }
        else if (props.aliasDetail["HTTPAlias"] !== "" && props.aliasDetail["HTTPAlias"] !== undefined) {
            state.error["alias"] = false;
        }

        this.setState({
            aliasDetail: Object.assign({}, props.aliasDetail),
            triedToSubmit: false,
            error: state.error
        });
    }

    onSettingChange(key, event) {
        let {state, props} = this;
        let aliasDetail = Object.assign({}, state.aliasDetail);

        if (aliasDetail[key] === "" && key === "HTTPAlias") {
            state.error["alias"] = true;
        }
        else if (aliasDetail[key] !== "" && key === "HTTPAlias") {
            state.error["alias"] = false;
        }

        if (key === "BrowserType" || key === "CultureCode" || key === "Skin") {
            aliasDetail[key] = event.value;
        }
        else {
            aliasDetail[key] = typeof (event) === "object" ? event.target.value : event;
        }

        this.setState({
            aliasDetail: aliasDetail,
            triedToSubmit: false,
            error: state.error
        });

        props.dispatch(SiteSettingsActions.siteAliasClientModified(aliasDetail));
    }

    getBrowserOptions() {
        let options = [];
        if (this.props.browsers !== undefined) {
            options = this.props.browsers.map((item) => {
                return { label: item, value: item };
            });
        }
        return options;
    }

    getLanguageOptions() {
        let options = [];
        const noneSpecifiedText = "<" + resx.get("NoneSpecified") + ">";
        if (this.props.languages !== undefined) {
            options = this.props.languages.map((item) => {
                return { label: item.Key, value: item.Value };
            });
            options.unshift({ label: noneSpecifiedText, value: "" });
        }
        return options;
    }

    getSkinOptions() {
        let options = [];
        const noneSpecifiedText = "<" + resx.get("NoneSpecified") + ">";
        if (this.props.skins !== undefined) {
            options = this.props.skins.map((item) => {
                return { label: item.Key, value: item.Value };
            });
            options.unshift({ label: noneSpecifiedText, value: "" });
        }
        return options;
    }

    onSave(event) {
        const {props, state} = this;
        this.setState({
            triedToSubmit: true
        });
        if (state.error.alias) {
            return;
        }

        props.onUpdate(state.aliasDetail);
    }

    onSetPrimary(event) {
        const {props, state} = this;
        this.setState({
            triedToSubmit: true
        });
        if (state.error.alias) {
            return;
        }
        let aliasDetail = Object.assign({}, state.aliasDetail);
        aliasDetail["IsPrimary"] = true;
        this.setState({
            aliasDetail: aliasDetail
        }, () => {
            props.onUpdate(aliasDetail);
        });
    }

    onCancel(event) {
        const {props, state} = this;
        if (props.siteAliasClientModified) {
            util.utilities.confirm(resx.get("SettingsRestoreWarning"), resx.get("Yes"), resx.get("No"), () => {
                props.dispatch(SiteSettingsActions.cancelSiteAliasClientModified());
                props.Collapse();
            });
        }
        else {
            props.Collapse();
        }
    }

    /* eslint-disable react/no-danger */
    render() {
        /* eslint-disable react/no-danger */
        if (this.state.aliasDetail !== undefined || this.props.id === "add") {
            const columnOne = <div className="left-column">
                <InputGroup>
                    <Label
                        label={resx.get("SiteAlias")}
                        />
                    <SingleLineInputWithError
                        inputStyle={{ margin: "0" }}
                        withLabel={false}
                        error={this.state.error.alias && this.state.triedToSubmit}
                        errorMessage={resx.get("InvalidAlias")}
                        value={this.state.aliasDetail.HTTPAlias}
                        onChange={this.onSettingChange.bind(this, "HTTPAlias")}
                        />
                </InputGroup>
                <InputGroup>
                    <Label
                        label={resx.get("Browser")}
                        />
                    <Dropdown
                        options={this.getBrowserOptions()}
                        value={this.state.aliasDetail.BrowserType}
                        onSelect={this.onSettingChange.bind(this, "BrowserType")}
                        />
                </InputGroup>
            </div>;
            const columnTwo = <div className="right-column">
                {this.props.languages.length > 1 &&
                    <InputGroup>
                        <Label
                            label={resx.get("Language")}
                            />
                        <Dropdown
                            options={this.getLanguageOptions()}
                            value={this.state.aliasDetail.CultureCode}
                            onSelect={this.onSettingChange.bind(this, "CultureCode")}
                            />
                    </InputGroup>
                }
                <InputGroup>
                    <Label
                        label={resx.get("Theme")}
                        />
                    <Dropdown
                        options={this.getSkinOptions()}
                        value={this.state.aliasDetail.Skin}
                        onSelect={this.onSettingChange.bind(this, "Skin")}
                        />
                </InputGroup>
            </div>;

            return (
                <div className="alias-editor">
                    <Grid children={[columnOne, columnTwo]} numberOfColumns={2} />
                    <div className="editor-buttons-box">
                        <Button
                            type="secondary"
                            onClick={this.onCancel.bind(this)}>
                            {resx.get("Cancel")}
                        </Button>
                        <Button
                            disabled={this.state.aliasDetail.IsPrimary }
                            type="secondary"
                            onClick={this.onSetPrimary.bind(this)}>
                            {resx.get("SetPrimary")}
                        </Button>
                        <Button
                            type="primary"
                            onClick={this.onSave.bind(this)}>
                            {resx.get("Save")}
                        </Button>
                    </div>
                </div>
            );
        }
        else return <div />;
    }
}

SiteAliasEditor.propTypes = {
    dispatch: PropTypes.func.isRequired,
    aliasDetail: PropTypes.object,
    aliasId: PropTypes.number,
    browsers: PropTypes.array,
    languages: PropTypes.array,
    skins: PropTypes.array,
    Collapse: PropTypes.func,
    onUpdate: PropTypes.func,
    id: PropTypes.string,
    siteAliasClientModified: PropTypes.bool
};

function mapStateToProps(state) {
    return {
        aliasDetail: state.siteSettings.aliasDetail,
        browsers: state.siteSettings.browsers,
        languages: state.siteSettings.languages,
        skins: state.siteSettings.skins,
        siteAliasClientModified: state.siteSettings.siteAliasClientModified
    };
}

export default connect(mapStateToProps)(SiteAliasEditor);