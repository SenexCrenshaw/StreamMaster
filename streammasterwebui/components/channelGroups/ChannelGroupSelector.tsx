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

  useEffect(() => {
    if (!originalInput || originalInput !== value) {
      setOriginalInput(value);
      if (value) {
        setInput(value);
        const found = channelGroupQuery.data?.find((x) => x.Name === value);
        if (found) {
          // setSelectedChannelGroup(found);
        }
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
        {/* <VideoStreamSetAutoSetEPGDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} /> */}
        {/* <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} /> */}
        {/* <VideoStreamEditDialog value={data} /> */}
        {/* <VideoStreamCopyLinkDialog value={data} />
        <VideoStreamSetTimeShiftDialog iconFilled={false} value={data} />
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetLogoFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamSetAutoSetEPGDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} />
        <VideoStreamEditDialog value={data} /> */}
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
      { field: 'Name', filter: true, sortable: true },
      { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionTemplate, streamCountTemplate]
  );

  // const dataSource = useMemo(() => {
  //   if (!channelGroupQuery.data || channelGroupQuery.data.length === 0) {
  //     return undefined;
  //   }

  //   if (sortInfo.sortField === 'ActiveCount') {
  //     const test = [...channelGroupQuery.data].sort((a, b) => {
  //       return sortInfo.sortOrder === 1 ? a.ActiveCount - b.ActiveCount : b.ActiveCount - a.ActiveCount;
  //     });
  //     console.log(test);

  //     return test;
  //   }
  //   if (sortInfo.sortField === 'TotalCount') {
  //     return [...channelGroupQuery.data].sort((a, b) => {
  //       return sortInfo.sortOrder === -1 ? a.TotalCount - b.TotalCount : b.TotalCount - a.TotalCount;
  //     });
  //   }

  //   return channelGroupQuery.data;
  // }, [channelGroupQuery.data, sortInfo]);

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
        <SMOverlay buttonTemplate={buttonTemplate} title="GROUPS" widthSize="3" icon="pi-chevron-down">
          <SMDataTable
            id={dataKey}
            columns={columns}
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
