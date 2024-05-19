import { SMOverlay } from '@components/sm/SMOverlay';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';
import useGetPagedChannelGroups from '@lib/smAPI/ChannelGroups/useGetPagedChannelGroups';
import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';

type ChannelGroupSelectorProperties = {
  readonly darkBackGround?: boolean;
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly label?: string;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const ChannelGroupSelector = ({
  enableEditMode = true,
  darkBackGround = false,
  label,
  value,
  disabled,
  editable,
  onChange
}: ChannelGroupSelectorProperties) => {
  const dataKey = `channelGroupSelector`;
  const [input, setInput] = useState<string | undefined>(undefined);
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const { isSystemReady } = useSMContext();

  const channelGroupQuery = useGetChannelGroups();
  const { columnConfig: channelGroupNameColumnConfig } = useChannelGroupNameColumnConfig({ enableEdit: true });

  useEffect(() => {
    if (!originalInput || originalInput !== value) {
      setOriginalInput(value);
      if (value) {
        setInput(value);
        // const found = channelGroupQuery.data?.find((x) => x.Name === value);
        // if (found) {
        //   // setSelectedChannelGroup(found);
        // }
      }
    }
  }, [value, originalInput, channelGroupQuery.data]);

  const loading = channelGroupQuery.isError || channelGroupQuery.isFetching || channelGroupQuery.isLoading || !channelGroupQuery.data || isSystemReady !== true;

  const buttonTemplate = useMemo(() => {
    if (input)
      return (
        <div className="sm-channelgroup-selector">
          <div className="text-container ">{input}</div>
        </div>
      );

    return (
      <div className="sm-channelgroup-selector ">
        <div className="text-container ">None</div>
      </div>
    );
  }, [input]);

  const actionTemplate = useCallback(
    (data: ChannelGroupDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <ChannelGroupVisibleDialog id={dataKey} value={data} />
        <ChannelGroupDeleteDialog id={dataKey} value={data} />
      </div>
    ),
    [dataKey]
  );

  const streamCountTemplate = useCallback((data: ChannelGroupDto) => {
    return (
      <div>
        {data.ActiveCount}/{data.TotalCount}
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelGroupNameColumnConfig,
      { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
  );

  const headerRightTemplate = useMemo(() => {
    return (
      <>
        <ChannelGroupVisibleDialog id={dataKey} />
        <ChannelGroupDeleteDialog id={dataKey} />
        <ChannelGroupAddDialog />
      </>
    );
  }, [dataKey]);

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0 text-container">{input ?? 'Dummy'}</div>;
  }

  return (
    <>
      <div className="stringeditor flex flex-column align-items-start">
        {label && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
            <div className="pt-small" />
          </>
        )}
      </div>
      <div className={darkBackGround ? 'sm-input-border-dark p-0 input-height' : 'p-0 input-height'}>
        <SMOverlay buttonTemplate={buttonTemplate} title="GROUPS" widthSize="3" icon="pi-chevron-down" header={headerRightTemplate}>
          <SMDataTable
            id={dataKey}
            columns={columns}
            noSourceHeader
            queryFilter={useGetPagedChannelGroups}
            rows={10}
            enablePaginator
            selectionMode="multiple"
            showHiddenInSelection
          />
        </SMOverlay>
      </div>
    </>
  );
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default React.memo(ChannelGroupSelector);
