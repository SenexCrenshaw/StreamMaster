
import { memo, useCallback, useEffect, useMemo, useRef, useState, type CSSProperties } from "react";
import { type DataTableFilterMetaData } from "../common/common";
import { getTopToolOptions } from "../common/common";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import ChannelGroupAddDialog from "./channelGroups/ChannelGroupAddDialog";
import { type ColumnMeta } from '../features/dataSelector2/DataSelectorTypes2';

import { useLocalStorage } from "primereact/hooks";
import DataSelector2 from "../features/dataSelector2/DataSelector2";
import { type DataTableFilterEvent } from "primereact/datatable";
import { type PagedResponseOfChannelGroupDto, type ChannelGroupDto, type ChannelGroupsGetChannelGroupsApiArg } from "../store/iptvApi";
import { useChannelGroupsGetChannelGroupsQuery } from "../store/iptvApi";
import ChannelGroupDeleteDialog from "./channelGroups/ChannelGroupDeleteDialog";
import ChannelGroupEditDialog from "./channelGroups/ChannelGroupEditDialog";
import ChannelGroupVisibleDialog from "./channelGroups/ChannelGroupVisibleDialog";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {
  const id = props.id + '-PlayListDataSelector';

  const [dataSource, setDataSource] = useState({} as PagedResponseOfChannelGroupDto);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, id + '-showHidden');

  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);

  const [pageSize, setPageSize] = useState<number>(25);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [filters, setFilters] = useState<string>('');
  const [orderBy, setOrderBy] = useState<string>('name asc');
  const timestampRef = useRef(Date.now()).current;
  const channelGroupsQuery = useChannelGroupsGetChannelGroupsQuery({ jsonArgumentString: timestampRef.toString(), jsonFiltersString: filters, orderBy: orderBy, pageNumber: pageNumber, pageSize: pageSize } as ChannelGroupsGetChannelGroupsApiArg);

  useEffect(() => {
    console.log('useEffect', orderBy);
  }, [orderBy]);

  useEffect(() => {
    if (channelGroupsQuery.data === undefined) {
      return;
    }

    if (!channelGroupsQuery.data?.data) {
      return;
    }

    setDataSource(channelGroupsQuery.data);

  }, [channelGroupsQuery.data]);


  const onSelectionChange = (data: ChannelGroupDto[]) => {
    setSelectedChannelGroups(data);
    props.onSelectionChange?.(data);
  };

  const onDelete = useCallback((results: string[] | undefined) => {
    if (results === undefined) {
      return;
    }

    const newSelectedChannelGroups = selectedChannelGroups.filter(
      group => !results.includes(group.name)
    );

    onSelectionChange(newSelectedChannelGroups);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const sourceActionBodyTemplate = useCallback((data: ChannelGroupDto) => (

    <div className='flex p-0 justify-content-end align-items-center'>

      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog iconFilled={false} onDelete={onDelete} value={[data]} />
      </div>

      <ChannelGroupEditDialog value={data} />

      <ChannelGroupVisibleDialog iconFilled={false} skipOverLayer value={[data]} />

    </div>

  ), [onDelete, props.useReadOnly]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
      {
        field: 'name', fieldType: 'streams', header: "Streams (active/total)",
        style: {
          maxWidth: '6rem',
          width: '6rem',
        } as CSSProperties,
      },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate, field: 'isHidden', fieldType: 'isHidden', header: 'Actions',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [sourceActionBodyTemplate]);

  const setFilter = useCallback((toFilter: DataTableFilterEvent): DataTableFilterMetaData[] => {

    if (toFilter === undefined || toFilter.filters === undefined) {
      return [] as DataTableFilterMetaData[];
    }

    const retData = [] as DataTableFilterMetaData[];
    Object.keys(toFilter.filters).forEach((key) => {
      const value = toFilter.filters[key] as DataTableFilterMetaData;
      if (value.value === null || value.value === undefined || value.value === '') {
        return;
      }

      const newValue = { ...value } as DataTableFilterMetaData;
      newValue.fieldName = key;
      retData.push(newValue);
    });


    setFilters(JSON.stringify(retData));
    return retData;
  }, []);

  const sourceRightHeaderTemplate = useCallback(() => {
    const getToolTip = (value: boolean | null | undefined) => {
      if (value === null) {
        return 'Show All';
      }

      if (value === true) {
        return 'Show Visible';
      }

      return 'Show Hidden';
    }

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">

        {props.hideControls !== true &&
          <>
            <TriStateCheckbox
              onChange={(e: TriStateCheckboxChangeEvent) => {
                setShowHidden(e.value);
              }}
              tooltip={getToolTip(showHidden)}
              tooltipOptions={getTopToolOptions}
              value={showHidden} />

            <ChannelGroupVisibleDialog value={selectedChannelGroups} />
            <ChannelGroupDeleteDialog onDelete={onDelete} value={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [props.hideControls, showHidden, selectedChannelGroups, onDelete, setShowHidden]);


  return (

    <DataSelector2
      columns={sourceColumns}
      dataSource={dataSource}
      emptyMessage="No Groups"
      headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
      hideControls={props.hideControls}
      id={id + 'DataSelector'}
      isLoading={channelGroupsQuery.isLoading || channelGroupsQuery.isFetching}
      name={props.name === undefined ? 'Playlist' : props.name}
      onFilter={(filterInfo) => {
        setFilter(filterInfo as DataTableFilterEvent);
      }}

      onPage={(pageInfo) => {

        if (pageInfo.page !== undefined) {
          setPageNumber(pageInfo.page + 1);
        }

        if (pageInfo.rows !== undefined) {
          setPageSize(pageInfo.rows);
        }
      }}

      onSelectionChange={(e) => {
        onSelectionChange(e as ChannelGroupDto[]);
      }}

      onSort={(e) => {
        console.log(e);
        setOrderBy(e);
      }
      }
      selectionMode='multiple'
      showHidden={showHidden}
      style={{
        height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
      }}
    />

  );
};

PlayListDataSelector.displayName = 'Play List Editor';
PlayListDataSelector.defaultProps = {
  hideAddRemoveControls: false,
  hideControls: false,
  maxHeight: null,
  name: 'Playlist',
  useReadOnly: true
};

export type PlayListDataSelectorProps = {
  hideAddRemoveControls?: boolean;
  hideControls?: boolean;
  id: string;
  maxHeight?: number;
  name?: string;
  onSelectionChange?: (value: ChannelGroupDto | ChannelGroupDto[]) => void;
  useReadOnly?: boolean;
};

export default memo(PlayListDataSelector);
