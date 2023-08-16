/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */

import React from "react";
import * as StreamMasterApi from '../../store/iptvApi';
import * as Hub from '../../store/signlar_functions';
import { Toast } from 'primereact/toast';
import DataSelector from "../dataSelector/DataSelector";
import { ColumnMeta } from "../dataSelector/DataSelectorTypes";
import PlayListDataSelector from "../../components/PlayListDataSelector";
import VideoStreamDataSelector from "../../components/VideoStreamDataSelector";

const TestPanel = (props: TestPanelProps) => {
  const toast = React.useRef<Toast>(null);

  const [dataSource, setDataSource] = React.useState({} as StreamMasterApi.PagedResponseOfChannelGroupDto);

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery({} as StreamMasterApi.PagedResponseOfChannelGroupDto);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
    ]
  }, []);

  return (
    <VideoStreamDataSelector id="testPanel" />
  );
}

TestPanel.displayName = 'TestPanel';
TestPanel.defaultProps = {
  onChange: null,
  value: null,
};

type TestPanelProps = {
  data?: StreamMasterApi.ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(TestPanel);
