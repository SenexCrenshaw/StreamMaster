/* eslint-disable @typescript-eslint/no-unused-vars */
import './DataSelector.css';

import { FilterMatchMode, FilterOperator } from 'primereact/api';
import { Button } from 'primereact/button';
import { Skeleton } from 'primereact/skeleton';

import { type ColumnSortEvent } from 'primereact/column';

import { Column } from 'primereact/column';
import { type DataTableFilterMetaData } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableRowToggleEvent } from 'primereact/datatable';
import { type DataTableSelectionChangeEvent } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';
import { type DataTableRowDataArray, } from 'primereact/datatable';
import { type DataTableRowData } from 'primereact/datatable';

import { DataTable } from 'primereact/datatable';
import * as StreamMasterApi from '../../store/iptvApi';
import { useLocalStorage } from 'primereact/hooks';
import { InputText } from 'primereact/inputtext';
import { type CSSProperties } from 'react';
import React from 'react';
import { camel2title, getTopToolOptions } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';
import { Checkbox } from 'primereact/checkbox';

import { Tooltip } from 'primereact/tooltip';
import { v4 as uuidv4 } from 'uuid';
import { type ColumnAlign, type ColumnFieldType, type ColumnMeta, type DataSelectorSelectionMode } from './DataSelectorTypes';
import { useIntl } from 'react-intl';
import { type VirtualScrollerChangeEvent } from 'primereact/virtualscroller';
import { type VirtualScrollerOptionsType } from 'primereact/virtualscroller';

const DataSelector = <T extends DataTableValue,>(props: DataSelectorProps<T>) => {
  const tableRef = React.useRef<DataTable<T[]>>(null);

  const tooltipClassName = React.useMemo(() => "menuitemds-" + uuidv4(), []);
  const [globalSourceFilterValue, setGlobalSourceFilterValue] = useLocalStorage('', props.id + '-sourceGlobalFilterValue');
  const [globalSearchName, setGlobalSearchName] = React.useState<string>('');

  const [dataSource, setDataSource] = React.useState<T[]>();

  const [expandedRows, setExpandedRows] = React.useState<DataTableExpandedRows>();

  const setting = StreamMasterSetting();
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, props.id + '-rowClick');

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();
  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery();

  const m3uFiles = StreamMasterApi.useM3UFilesGetM3UFilesQuery({ pageNumber: 0, pageSize: 25 } as StreamMasterApi.QueryStringParameters);

  const [selections, setSelections] = React.useState<T[]>([] as T[]);
  const intl = useIntl();

  const GetMessage = React.useCallback((id: string): string => {
    const message = intl.formatMessage({ id: id });

    return message;
  }, [intl]);


  React.useEffect(() => {
    if (props.globalSearchName !== null && props.globalSearchName !== undefined) {
      setGlobalSearchName(props.globalSearchName);
    } else {
      setGlobalSearchName(GetMessage('keywordSearch'));
    }

  }, [GetMessage, props.globalSearchName]);

  const isLoading = React.useMemo(() => {
    if (props.isLoading) {
      return true;
    }

    if (globalSourceFilterValue === undefined) {
      return true;
    }

    if (rowClick === undefined) {
      return true;
    }

    if (videoStreamsQuery.isLoading || !videoStreamsQuery.data) {
      return true;
    }

    if (channelGroupsQuery.isLoading || !channelGroupsQuery.data) {
      return true;
    }

    return false;

  }, [channelGroupsQuery.data, channelGroupsQuery.isLoading, globalSourceFilterValue, props.isLoading, rowClick, videoStreamsQuery.data, videoStreamsQuery.isLoading]);

  const showSkeleton = React.useMemo(() => {
    return isLoading || (props.showSkeleton !== undefined && props.showSkeleton)
  }, [isLoading, props.showSkeleton]);

  const sourceFilter = React.useMemo((): DataTableFilterMeta => {
    if (props.columns === undefined) {
      return {} as DataTableFilterMeta;
    }

    const global = {
      global: {
        matchMode: FilterMatchMode.CONTAINS,
        value: globalSourceFilterValue,
      }
    } as DataTableFilterMeta;

    const filterData = props.columns.reduce((obj, item: ColumnMeta) => {
      if (item.field === 'isHidden') {
        return {
          ...obj,
          [item.field]: {
            constraints: [{
              matchMode: FilterMatchMode.EQUALS,
              value: props.showHidden === null ? null : !props.showHidden
            }],
            operator: FilterOperator.AND
          },
        } as DataTableFilterMeta;
      }

      return {
        ...obj,
        [item.field]: {
          constraints: [{
            matchMode: item.filterMatchMode ?? FilterMatchMode.CONTAINS,
            value: ''
          }],
          operator: FilterOperator.AND
        },
      } as DataTableFilterMeta;

    }, {}) as DataTableFilterMeta;

    const toret = { ...global, ...filterData };

    return toret;

  }, [props.columns, props.showHidden, globalSourceFilterValue]);


  const [sourceFilters, setSourceFilters] = React.useState<DataTableFilterMeta>(sourceFilter);

  React.useMemo(() => {
    setSourceFilters(sourceFilter);

  }, [sourceFilter]);


  const getRecord = React.useCallback((data: T, fieldName: string) => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    return record;
  }, []);

  const getRecordString = React.useCallback((data: T, fieldName: string): string => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    let toDisplay = JSON.stringify(record);

    if (!toDisplay || toDisplay === undefined || toDisplay === '') {
      // console.log("toDisplay is empty for " + fieldName)
      return '';
    }

    if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
      toDisplay = toDisplay.substring(1, toDisplay.length - 1);
    }

    return toDisplay;
  }, []);

  const imageBodyTemplate = React.useCallback((data: T, fieldName: string) => {
    const record = getRecordString(data, fieldName);

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img
          alt={record ?? 'Logo'}
          className="max-h-1rem max-w-full p-0"
          onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  }, [getRecordString, setting.defaultIcon]);

  const clearSourceFilter = React.useCallback(() => {
    setSourceFilters(sourceFilter);
    setGlobalSourceFilterValue('');
  }, [setGlobalSourceFilterValue, sourceFilter]);

  const onGlobalSourceFilterChange = React.useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    const filtersToSet = { ...sourceFilters };

    filtersToSet.global = {
      matchMode: FilterMatchMode.CONTAINS,
      value: value
    } as DataTableFilterMetaData;


    setSourceFilters(filtersToSet);

    setGlobalSourceFilterValue(value);
  }, [setGlobalSourceFilterValue, sourceFilters]);

  const onValueChanged = React.useCallback((data: DataTableRowDataArray<T[]>) => {
    if (!data) {
      return;
    }

    props?.onValueChanged?.(data);

  }, [props]);

  const streamCount = React.useCallback((groupName: string) => {

    if (groupName === null || groupName === undefined || !videoStreamsQuery.data) {
      return 0;
    }

    if (groupName === 'All') {
      return videoStreamsQuery.data.length;
    }

    const cg = channelGroupsQuery.data?.find((x: StreamMasterApi.ChannelGroupDto) => x.name.toLowerCase() === groupName.toLowerCase());
    if (cg?.regexMatch !== undefined && cg.regexMatch !== '') {
      const filteredData = videoStreamsQuery.data.filter((item) => {

        const regexToTest = new RegExp(`.*${cg.regexMatch}.*`, 'i');
        return regexToTest.test(item.user_Tvg_name);
      });

      const goodLength = videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase()).length;

      return goodLength + filteredData.length;
    }

    if (props.m3uFileId !== undefined && props.m3uFileId > 0) {
      return videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.m3UFileId === props.m3uFileId && x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase()).length;
    }

    return videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase()).length;

  }, [videoStreamsQuery.data, channelGroupsQuery.data, props.m3uFileId]);

  const streamNotHiddenCount = React.useCallback((groupName: string) => {
    if (groupName === null || groupName === undefined || !videoStreamsQuery.data) {
      return 0;
    }

    if (groupName === 'All') {
      return videoStreamsQuery.data.length;
    }

    const cg = channelGroupsQuery.data?.find((x: StreamMasterApi.ChannelGroupDto) => x.name.toLowerCase() === groupName.toLowerCase());
    if (cg?.regexMatch !== undefined && cg.regexMatch !== '') {
      const filteredData = videoStreamsQuery.data.filter((item) => {
        if (item.isHidden) {
          return false;
        }

        const regexToTest = new RegExp(`.*${cg.regexMatch}.*`, 'i');
        return regexToTest.test(item.user_Tvg_name);
      });

      const goodLength = videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase() && !x.isHidden).length;

      return goodLength + filteredData.length;
    }

    if (props.m3uFileId !== undefined && props.m3uFileId > 0) {
      return videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.m3UFileId === props.m3uFileId && x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase() && !x.isHidden).length;
    }

    return videoStreamsQuery.data.filter((x: StreamMasterApi.VideoStreamDto) => x.user_Tvg_group !== null && x.user_Tvg_group.toLowerCase() === groupName.toLowerCase() && !x.isHidden).length;

  }, [videoStreamsQuery.data, channelGroupsQuery.data, props.m3uFileId]);


  const streamsBodyTemplate = React.useCallback((groupName: string) => {
    if (groupName === null || groupName === undefined || !videoStreamsQuery.data) {
      return 0;
    }

    return (
      <div className="flex align-items-center gap-2" >
        {streamNotHiddenCount(groupName)}/{streamCount(groupName)}
      </div>
    );

  }, [videoStreamsQuery.data, streamCount, streamNotHiddenCount]);

  const m3uFileNameBodyTemplate = React.useCallback((id: number) => {
    if (!id || id === 0 || !m3uFiles || !m3uFiles.data) {
      return (
        <div className="flex align-items-center gap-2" >
          User Created
        </div>
      );
    }

    const m3uFile = m3uFiles.data.find((x) => x.id === id);

    return (
      <div className="flex align-items-center gap-2" >
        {m3uFile?.name}
      </div>
    );
  }, [m3uFiles]);

  const linkIcon = (url: string) => {
    return (
      <a href={url} rel="noopener noreferrer" target="_blank">
        <i className="pi pi-bookmark-fill" />
      </a>
    );
  };

  const m3uLinkSourceTemplate = React.useCallback((link: string) => {
    return (
      <div>
        <div className="flex justify-content-center align-items-center">
          {linkIcon(link)}
        </div>
      </div >
    );
  }, []);

  const epgLinkSourceTemplate = React.useCallback((link: string) => {
    return (
      <div>
        <div className="flex justify-content-center align-items-center">
          {linkIcon(link)}
        </div>
      </div>
    );
  }, []);

  const urlLinkSourceTemplate = React.useCallback((link: string) => {
    return (
      <div>
        <div className="flex justify-content-center align-items-center">
          {linkIcon(link)}
        </div>
      </div>
    );
  }, []);

  const epgSourceTemplate = React.useCallback((tvgid: string) => {
    return (
      <div>
        <div className="flex justify-content-start">
          {tvgid}
        </div>
      </div>
    );
  }, []);

  const bodyTemplate = React.useCallback((data: T, fieldName: string, fieldType: ColumnFieldType, camelize: boolean | undefined) => {
    if (fieldType === 'isHidden') {
      camelize = true;
    }

    if (fieldType === 'blank') {
      return (<div />);
      ;
    }

    if (fieldType === 'm3uFileName') {
      const m3UFileId = getRecordString(data, 'm3UFileId');
      return m3uFileNameBodyTemplate(parseInt(m3UFileId));
    }

    if (fieldType === 'm3ulink') {
      const link = getRecordString(data, 'm3ULink');
      return m3uLinkSourceTemplate(link);
    }

    if (fieldType === 'epglink') {
      const link = getRecordString(data, 'xmlLink');
      return epgLinkSourceTemplate(link);
    }

    if (fieldType === 'url') {
      const link = getRecordString(data, 'hdhrLink');
      return urlLinkSourceTemplate(link);
    }

    if (fieldType === 'epg') {
      const tvgid = getRecordString(data, 'user_Tvg_ID');
      return epgSourceTemplate(tvgid);
    }

    if (fieldType === 'image') {
      return imageBodyTemplate(data, fieldName);
    }

    if (fieldType === 'streams') {
      const recordstr = getRecordString(data, fieldName);
      return streamsBodyTemplate(recordstr);
    }

    const record = getRecord(data, fieldName);
    if (record === undefined) return (<div />);

    if (fieldType === 'isHidden') {
      if (record !== true) {
        return (
          <i className="pi pi-eye text-green-500" />
        )
      }

      return (
        <i className="pi pi-eye-slash text-red-500" />
      )
    }

    let toDisplay = JSON.stringify(record);

    if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
      toDisplay = toDisplay.substring(1, toDisplay.length - 1);
    }

    if (fieldType === 'deleted') {
      const isDeleted = getRecord(data, 'isHidden');
      if (isDeleted !== true) {
        return (
          <span className="flex bg-green-900 min-w-full min-h-full justify-content-center align-items-center" >
            {toDisplay}
          </span>
        )
      }

      return (
        <span className="flex bg-red-900 min-w-full min-h-full justify-content-center align-items-center" >
          {toDisplay}
        </span>
      )
    }

    if (camelize === true) {
      toDisplay = camel2title(toDisplay);
    }

    return (
      <span
        style={{
          ...{
            display: 'block',
            overflow: 'hidden',
            padding: '0rem !important',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          },
        }}
      >
        {toDisplay}
      </span>
    );
  }, [epgLinkSourceTemplate, epgSourceTemplate, getRecord, getRecordString, imageBodyTemplate, m3uFileNameBodyTemplate, m3uLinkSourceTemplate, streamsBodyTemplate, urlLinkSourceTemplate]);

  React.useEffect(() => {
    if (props.dataSource !== undefined) {
      setDataSource(props.dataSource);
      // console.debug("Set Data Source", props.dataSource.length);


      // if (props.dataSource.length < 4) {
      //   console.debug("Set Data Source", props.dataSource);
      // }

    }

  }, [props.dataSource]);

  React.useEffect(() => {

    if (dataSource !== undefined && props.enableVirtualScroll === true && dataSource.length > 0) {
      // console.debug("Scroll to ", dataSource.length);

      if (tableRef.current?.getVirtualScroller()?.scrollToIndex !== undefined) {
        tableRef.current.getVirtualScroller().scrollToIndex(10, 'auto');
      }
    }

  }, [dataSource, props.enableVirtualScroll]);

  const showPagination = React.useMemo((): boolean => {

    if (props.enableVirtualScroll === true) {
      return false;
    }

    if (props.showPagination !== true) {
      return false;
    }

    let dataLength = 0;

    // if (values && values.length > 0) {
    //   dataLength = values.length;
    // } else {
    if (dataSource && dataSource.length > 0) {
      dataLength = dataSource.length;
    }
    else {
      return false;
    }
    // }


    const minRows = props.paginatorMinimumRowsToShow ? props.paginatorMinimumRowsToShow : 20;

    return dataLength >= minRows;

  }, [props.enableVirtualScroll, props.showPagination, props.paginatorMinimumRowsToShow, dataSource]);

  const onRowReorder = React.useCallback((data: T[]) => {
    setDataSource(data);
    props.onSelectionChange?.(data);
  }, [props]);


  const rowClass = React.useCallback((data: DataTableRowData<T[]>) => {

    const isHidden = getRecord(data as T, 'isHidden');

    if (getRecord(data as T, 'regexMatch') === '') {

      const groupName = getRecord(data as T, 'name');

      if (groupName !== undefined && groupName !== '') {
        if (streamNotHiddenCount(groupName) > 0) {
          return {};
        }
      }
    }

    if (isHidden === true) {
      return `bg-red-900`;
    }

    return {};
  }, [getRecord, streamNotHiddenCount]);


  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };



  const sourceRenderHeader = React.useMemo(() => {
    if (!props.headerLeftTemplate && !props.headerRightTemplate && !props.globalSearchEnabled) {
      return null;
    }

    return (
      <div className="flex flex-row w-full flex-wrap grid align-items-center w-full col-12 h-full p-0 debug">
        <div className="flex col-2 text-orange-500 h-full text-sm align-items-center p-0 debug" >
          {props.name}
          <div hidden={props.selectionMode !== 'selectable'}>
            {showSkeleton && <Skeleton height="1rem" width="2rem" />}
            {showSkeleton !== true &&
              <Checkbox
                checked={rowClick}
                onChange={(e) => {
                  props?.onMultiSelectClick?.(e.checked ?? false);
                  setRowClick(e.checked ?? false);
                }
                }
                tooltip="Multi Select"
                tooltipOptions={getTopToolOptions}
              />
            }
          </div>
        </div>
        <div className="flex flex-wrap col-10 h-full justify-contents-between align-items-center p-0 m-0 debug">
          <div className="grid mt-2 flex flex-nowrap flex-row justify-content-between align-items-center col-12 px-0">

            <div className={`flex debug flex-nowrap justify-content-start header p-0 m-0 align-items-center ${props?.headerLeftTemplate !== undefined ? `col-${props.leftColSize !== undefined ? props.leftColSize : 4}` : 'col-1'}`}>

              {props.headerLeftTemplate ?
                showSkeleton ? <Skeleton className="mb-2" height="1.5rem" />
                  :
                  props.headerLeftTemplate
                : null}
            </div >

            <div className={`flex emptyheader h-full p-0 m-0 justify-content-start align-items-center debugBlue ${props?.headerLeftTemplate !== undefined ? `col-${12 - (props.leftColSize !== undefined ? props.leftColSize : 4)}` : 'col-11'}`}>
              <div className="grid flex-nowrap align-items-center justify-content-between col-12 debug" >
                <div className={`col-${props.rightColSize !== undefined ? 12 - props.rightColSize : '6'} flex debug p-0 m-0 justify-content-start align-items-center`}>
                  {showSkeleton ?
                    <>
                      <Skeleton className="mb-2" height="1.5rem" />
                      <Skeleton className="mb-2" height="1.5rem" />
                    </>
                    :
                    <>
                      <Button
                        className="p-button-text"
                        hidden={props.columns === undefined || props.columns.length === 0}
                        icon="pi pi-filter-slash"
                        onClick={clearSourceFilter}
                        rounded
                        text
                        tooltip="Clear Filter"
                        tooltipOptions={getTopToolOptions}
                        type="button"
                      />
                      <InputText
                        className="withpadding flex w-full"
                        disabled={props.columns === undefined || props.columns.length === 0}
                        hidden={props.columns === undefined || props.columns.length === 0}
                        onChange={((e) => onGlobalSourceFilterChange(e))}
                        placeholder={globalSearchName}
                        value={globalSourceFilterValue ?? ''}
                      />


                    </>
                  }
                </div>

                <div className={`flex col-${props.rightColSize !== undefined ? props.rightColSize : '6'} debug p-0 m-0 justify-content-end align-items-center`} >
                  {props.enableExport === true &&
                    <Button
                      className="p-button-text justify-content-end"
                      data-pr-tooltip="CSV"
                      icon="pi pi-file-export"
                      onClick={() => exportCSV()}
                      rounded
                      text
                      tooltip="Clear Filter"
                      tooltipOptions={getTopToolOptions}
                      type="button"
                    />
                  }
                  {showSkeleton ?
                    <Skeleton className="mb-2" height="1.5rem" />
                    :
                    props.headerRightTemplate
                  }
                </div>

              </div>
            </div >
          </div >
        </div>
      </div>
    );
  }, [clearSourceFilter, globalSearchName, globalSourceFilterValue, onGlobalSourceFilterChange, props, rowClick, setRowClick, showSkeleton]);

  const onsetSelection = React.useCallback((e: T | T[]): T | T[] | undefined => {

    if (props.selectionMode === 'single') {
      const data = e as T[];

      if (data.length === 0) {
        setSelections([] as T[]);

        if (props.onSelectionChange) {
          props.onSelectionChange({} as T)
        }

        return;
      }

      // const single1 = data.slice(data.length - 1, data.length);
      setSelections(data);
      if (props.onSelectionChange) {
        props.onSelectionChange(data[0])
        return;
      }

    }

    setSelections(e as T[]);
    if (props.onSelectionChange) {
      props.onSelectionChange(e as T[]);
    }

    return e;

  }, [props]);


  const getSelectionMode = React.useMemo((): 'checkbox' | 'multiple' | 'radiobutton' | 'single' | undefined => {

    if (props.selectionMode === 'selectable') {
      if (!rowClick) {
        return 'single';
      }

      return 'multiple';
    }

    if (props.selectionMode === 'multipleNoCheckBox') {
      return 'multiple';
    }

    if (props.selectionMode === 'multipleNoRowCheckBox') {
      return 'radiobutton';
    }

    return props.selectionMode

  }, [props.selectionMode, rowClick]);

  React.useMemo(() => {
    if (props.selection === undefined) {
      return;
    }

    if (props.selection instanceof Array) {
      // console.log('useEffect', props.selection);
      setSelections(props.selection as T[]);
    } else {
      setSelections([props.selection as T]);
    }

  }, [props.selection])

  const onSelectionChange = React.useCallback((e: DataTableSelectionChangeEvent<T[]>) => {
    if (e.value === null || e.value === undefined) {
      return;
    }


    if (getSelectionMode === 'single' && e.value instanceof Array && e.value.length > 0) {
      const single1 = e.value.slice(e.value.length - 1, e.value.length);
      onsetSelection(single1);
    } else {
      onsetSelection(e.value as T[]);
    }

  }, [getSelectionMode, onsetSelection]);

  const getAlign = React.useCallback((align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {

    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (align === undefined || align === null) {
      return 'left'
    }

    return align;

  }, []);

  const getAlignHeader = React.useCallback((align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (!align) {
      return 'center'
    }

    return align;
  }, []);

  const getFilter = React.useCallback((filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  }, [])

  const sortFunction = React.useCallback((event: ColumnSortEvent) => {

    const sortedObjects: T[] = [];

    const idsToMoveToTop = selections.map((obj) => obj.id);

    if (event.order === 1) {

      // First, add any objects whose IDs are in the "idsToMoveToTop" list to the front of the sorted array
      for (const id of idsToMoveToTop) {
        const matchingObject = event.data.find((obj: T) => obj.id === id);
        if (matchingObject) {

          sortedObjects.push(matchingObject);
        }
      }

      // Then, add the remaining objects to the sorted array (excluding any that were already added in the previous loop)
      for (const obj of event.data) {
        if (!idsToMoveToTop.includes(obj.id)) {
          sortedObjects.push(obj);
        }
      }
    } else {
      // Then, add the remaining objects to the sorted array (excluding any that were already added in the previous loop)
      for (const obj of event.data) {
        if (!idsToMoveToTop.includes(obj.id)) {
          sortedObjects.push(obj);
        }
      }

      for (const id of idsToMoveToTop) {
        const matchingObject = event.data.find((obj: T) => obj.id === id);
        if (matchingObject) {
          sortedObjects.push(matchingObject);
        }
      }
    }

    return sortedObjects;
  }, [selections]);

  const getStyle = React.useCallback((style: CSSProperties | undefined, fieldType: ColumnFieldType | undefined): CSSProperties | undefined => {

    if (fieldType === 'blank') {
      return {
        maxWidth: '1rem',
        width: '1rem',
      } as CSSProperties;
    }

    if (fieldType === 'image' || fieldType === 'm3ulink' || fieldType === 'epglink' || fieldType === 'url') {
      return {
        maxWidth: '5rem',
        width: '5rem',
      } as CSSProperties;
    }

    return {
      ...style,
      flexGrow: 0,
      flexShrink: 1,
      // maxWidth: '10rem',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',

    } as CSSProperties;
  }, []);

  const rowGroupHeaderTemplate = React.useCallback((data: T) => {
    if (!props.groupRowsBy) {
      return (
        <div />
      );
    }

    const record = getRecordString(data, props.groupRowsBy);
    return (
      <span className="vertical-align-middle ml-2 font-bold line-height-3">
        {record}
        <Button
          className={data.isHidden ? 'p-button-danger' : ''}
          icon={`pi pi-eye ${data.isHidden ? 'text-red-500' : 'text-green-500'}`}
          onClick={() => props?.onRowVisibleClick?.(data)}
          rounded
          text
          tooltip="Set Hidden"
          tooltipOptions={getTopToolOptions}
        />
      </span>
    );
  }, [getRecordString, props]);

  const getHeader = React.useCallback((field: string, header: string | undefined, fieldType: ColumnFieldType | undefined): React.ReactNode => {
    if (fieldType) {
      if (fieldType === 'blank') { return (<div />); }

      if (fieldType === 'epg') { return 'EPG'; }

      if (fieldType === 'm3ulink') { return 'M3U'; }

      if (fieldType === 'epglink') { return 'XMLTV'; }

      if (fieldType === 'url') { return 'HDHR URL'; }
    }

    if (fieldType) {
      if (fieldType === 'streams') {

        return (
          <>
            <Tooltip target={"." + tooltipClassName} />
            <div
              className={tooltipClassName + " border-white"}
              data-pr-at="right+5 top"
              data-pr-hidedelay={100}
              data-pr-my="left center-2"
              data-pr-position="right"
              data-pr-showdelay={200}
              data-pr-tooltip="Active/Total Count"
            >
              Streams<br />(active/total)
            </div>
          </>
        )

      }
    }

    if (header === undefined) {
      return camel2title(field)
    }

    return header;

  }, [tooltipClassName]);

  const items = React.useMemo(() => {
    return [
      {
        id: 1,
        name: 1,
      } as DataTableValue,
      {
        id: 2,
        name: 2,
      } as DataTableValue,
      {
        id: 3,
        name: 3,
      } as DataTableValue, {
        id: 4,
        name: 4,
      } as DataTableValue,
      {
        id: 5,
        name: 5,
      } as DataTableValue,
      {
        id: 1,
        name: 1,
      } as DataTableValue,
      {
        id: 2,
        name: 2,
      } as DataTableValue,
      {
        id: 3,
        name: 3,
      } as DataTableValue, {
        id: 4,
        name: 4,
      } as DataTableValue,
      {
        id: 5,
        name: 5,
      } as DataTableValue
    ]
  }, []);

  const multiselectHeader = () => {
    return (
      <div className="absolute top-0 left-50 text-xs text-white text-500" />
    );
  }

  if (showSkeleton) {
    return (
      <div className='dataselector flex justify-content-start align-items-center' >
        <div className={`${props.className !== undefined ? props.className : ''} flex min-w-full min-h-full surface-overlay`}>

          <DataTable className="p-datatable-striped" header={sourceRenderHeader} value={items.concat(items)}>
            <Column
              body={<Skeleton />}
              className='max-w-1rem p-0 justify-content-center align-items-center'
              field='selector'
              header=""
              hidden={!props.showSelector}
              style={{ width: '1rem' }}
            />
            <Column
              body={<Skeleton />}
              className='max-w-2rem p-0 justify-content-center align-items-center'
              field='rank'
              hidden={!props.reorderable}
              rowReorder
              style={{ width: '2rem' }}
            />
            <Column
              align='center'
              alignHeader='center'
              body={<Skeleton />}
              className='max-w-3rem p-0'
              field='getSelectionMode'
              headerStyle={{ width: '3rem' }}
              hidden={getSelectionMode === 'single'}
            />
            {props.columns.map((col) => (
              <Column
                align={getAlign(col.align, col.fieldType)}
                alignHeader={getAlignHeader(col.align, col.fieldType)}
                body={<Skeleton />}

                header={getHeader(col.field, col.header, col.fieldType)}
                hidden={col.isHidden ?? false}
                key={!col.fieldType ? col.field : col.field + col.fieldType}
                style={getStyle(col.style, col.fieldType)}
              />

            ))}
          </DataTable>

        </div>
      </div >
    );
  }

  return (

    <div className='dataselector flex w-full min-w-full  justify-content-start align-items-center' >
      <div className={`${props.className !== undefined ? props.className : ''} min-h-full w-full surface-overlay`}>
        <DataTable
          dataKey='id'
          editMode='cell'
          emptyMessage={props.emptyMessage}
          expandableRowGroups={props.groupRowsBy !== undefined && props.groupRowsBy !== ''}
          expandedRows={expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={1000}
          filters={sourceFilters}
          first={props.first}
          globalFilterFields={props.columns.map((item) => item.field)}
          groupRowsBy={props.groupRowsBy}
          header={sourceRenderHeader}
          key={props.key !== undefined && props.key !== '' ? props.key : undefined}
          loading={isLoading}
          metaKeySelection={false}
          onRowReorder={(e) => onRowReorder(e.value)}
          onRowToggle={(e: DataTableRowToggleEvent) => setExpandedRows(e.data as DataTableExpandedRows)}
          onSelectionChange={((e) => onSelectionChange(e))}
          onValueChange={(e) => { onValueChanged(e); }}
          paginator={showPagination}
          paginatorClassName='text-xs p-0 m-0 withpadding'
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableRef}
          removableSort
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
          rowGroupHeaderTemplate={rowGroupHeaderTemplate}
          rowGroupMode={props.groupRowsBy !== undefined && props.groupRowsBy !== '' ? 'subheader' : undefined}
          rows={25}
          rowsPerPageOptions={[25, 50, 100, 250]}
          scrollHeight={props.enableVirtualScroll === true ? props.virtualScrollHeight !== undefined ? props.virtualScrollHeight : '400px' : 'flex'}
          scrollable
          selection={selections}
          selectionMode={getSelectionMode}
          showGridlines
          showHeaders={props.showHeaders}
          sortField={props.groupRowsBy === undefined || props.groupRowsBy === '' ? props.sortField : props.groupRowsBy}
          sortMode='single'
          sortOrder={0}
          stateKey={props.enableState !== true ? undefined : props.id + '-datatable'}
          stateStorage={props.enableState !== true ? undefined : "local"}
          stripedRows
          style={props.style}
          value={dataSource}
          virtualScrollerOptions={props.enableVirtualScroll === true ? { itemSize: 16, orientation: 'vertical' } : undefined}
        >
          <Column
            body={<i className="pi pi-chevron-right" />}
            className='max-w-1rem p-0 justify-content-center align-items-center'
            field='selector'
            header=""
            hidden={!props.showSelector}
            style={{ width: '1rem' }}
          />
          <Column
            className='max-w-2rem p-0 justify-content-center align-items-center'
            field='rank'
            hidden={!props.reorderable}
            rowReorder
            style={{ width: '2rem' }}
          />
          <Column
            align='center'
            alignHeader='center'
            className={`justify-content-center align-items-center multiselect ${props.selectionMode}`}
            field='getSelectionMode'
            header={multiselectHeader}
            headerStyle={{ padding: '0px', width: '3rem' }}
            hidden={props.selectionMode !== 'multiple' && props.selectionMode !== 'checkbox' && props.selectionMode !== 'multipleNoRowCheckBox'}
            selectionMode="multiple"
            sortField="selected"
            sortFunction={sortFunction}
            sortable={props.groupRowsBy === undefined || props.groupRowsBy === ''}
          />
          {props.columns.map((col) => (
            <Column
              align={getAlign(col.align, col.fieldType)}
              alignHeader={getAlignHeader(col.align, col.fieldType)}
              body={((e) => col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, col.camelize))}
              editor={col.editor}
              field={col.field}
              filter={getFilter(col.filter, col.fieldType)}
              filterPlaceholder={col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)}
              header={getHeader(col.field, col.header, col.fieldType)}
              hidden={col.isHidden === true || (props.hideControls === true && getHeader(col.field, col.header, col.fieldType) === 'Actions') ? true : undefined}
              key={!col.fieldType ? col.field : col.field + col.fieldType}
              onCellEditComplete={col.handleOnCellEditComplete}
              showAddButton={false}
              showApplyButton={false}
              showClearButton={col.filterType !== 'isHidden'}
              showFilterMatchModes={false}
              showFilterMenu
              showFilterMenuOptions={false}
              showFilterOperator={false}
              sortable={props.groupRowsBy === undefined || props.groupRowsBy === '' ? col.sortable : false}
              style={getStyle(col.style, col.fieldType)}
            />

          ))}
        </DataTable>
      </div>
    </div >

  );
};

DataSelector.displayName = 'DataSelector';
DataSelector.defaultProps = {
  enableState: true,
  enableVirtualScroll: false,
  first: 0,
  globalSearchEnabled: true,
  hideControls: false,
  key: undefined,
  leftColSize: 4,
  name: '',
  onSelectionChange: undefined,
  paginatorMinimumRowsToShow: 20,
  reorderable: false,
  rightColSize: 8,
  selectionMode: 'single',
  showHeaders: true,
  showHidden: null,
  showPagination: true
};


/**
 * The props for the DataSelector component.
 *
 * @typeparam T The type of data being displayed in the table.
 */
export type DataSelectorProps<T> = {

  /**
   * The CSS class name for the component.
   */
  className?: string;
  /**
   * An array of ColumnMeta objects that define the columns of the table.
   */
  columns: ColumnMeta[];
  /**
   * An array of objects to display.
   */
  dataSource: T[] | undefined;
  /**
   * A React node that is displayed when there is no data to display.
   */
  emptyMessage?: React.ReactNode;

  enableExport?: boolean;
  // eslint-disable-next-line react/no-unused-prop-types
  enableState?: boolean | undefined;
  enableVirtualScroll?: boolean | undefined;
  exportFilename?: string;
  first?: number | undefined;
  /**
   * Whether to enable global searching.
   */
  globalSearchEnabled?: boolean;
  /**
   * The name of the global search.
   */
  globalSearchName?: string;
  /**
   * The name of the field to group the rows by.
   */
  groupRowsBy?: string;
  /**
   * A React node that can be used to display additional content in the left side of the header of the table.
   */
  headerLeftTemplate?: React.ReactNode;
  /**
   * A React node that can be used to display additional content in the right side of the header of the table.
   */
  headerRightTemplate?: React.ReactNode;
  hideControls?: boolean;
  /**
   * The unique identifier of the component.
   */
  id: string;
  /**
   * Whether the component is currently loading data.
   */
  isLoading?: boolean;
  key?: string | undefined;
  leftColSize?: number;
  m3uFileId?: number;
  /**
   * The name of the component.
   */
  name?: string;
  /**
   * A function that is called when the multi-select button is clicked.
   */
  onMultiSelectClick?: (value: boolean) => void;

  /**
   * A function that is called when a row's visibility is changed.
   */
  onRowVisibleClick?: (value: T) => void;
  /**
   * A function that is called when a row is selected.
   */
  onSelectionChange?: (value: T | T[]) => void;
  /**
     * A function that is called when the value changes.
     */
  onValueChanged?: (value: T[]) => void;
  paginatorMinimumRowsToShow?: number;

  /**
   * Whether rows can be reordered.
   */
  reorderable?: boolean;
  rightColSize?: number;
  /**
 * The currently selected row(s).
 */
  selection?: T | T[];
  /**
   * The mode for row selection.
   */
  selectionMode?: DataSelectorSelectionMode;
  /**
   * Whether to show the selector column.
   */
  showHeaders?: boolean | undefined;
  showHidden?: boolean | null | undefined;
  showPagination?: boolean;
  showSelector?: boolean;
  showSkeleton?: boolean;
  /**
   * The field to sort the data by.
   */
  sortField?: string;
  /**
   * The inline style of the component.
   */
  style?: CSSProperties;
  virtualScrollHeight?: string | undefined;
}

export default React.memo(DataSelector);
