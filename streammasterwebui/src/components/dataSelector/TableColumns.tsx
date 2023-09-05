import { Column } from "primereact/column";
import { camel2title, removeQuotes } from "../../common/common";
import { type ColumnAlign, type ColumnFieldType } from "./DataSelectorTypes";
import { type ColumnMeta } from "./DataSelectorTypes";
import { type SyntheticEvent } from "react";
import { type CSSProperties, type ReactNode } from "react";
import { useCallback } from "react";
import { Tooltip } from "primereact/tooltip";
import { v4 as uuidv4 } from 'uuid';
import StreamMasterSetting from "../../store/signlar/StreamMasterSetting";
import { type DataTableValue } from "primereact/datatable";

type TableColumnsProps = {
  columns: ColumnMeta[];
  groupRowsBy?: string;
  hideControls?: boolean;
};

export const TableColumns = <T extends DataTableValue,>({ columns, groupRowsBy, hideControls }: TableColumnsProps) => {
  const setting = StreamMasterSetting();

  console.log("TableColumns", columns)

  const tooltipClassName = () => "menuitemds-" + uuidv4();

  const getAlign = (align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {

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

  };

  const getAlignHeader = (align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
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
  };

  const getFilter = (filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  };

  const getHeader = (field: string, header: string | undefined, fieldType: ColumnFieldType | undefined): ReactNode => {
    if (!fieldType) {
      return header ? header : camel2title(field);
    }

    switch (fieldType) {
      case 'blank':
        return <div />;
      case 'epg':
        return 'EPG';
      case 'm3ulink':
        return 'M3U';
      case 'epglink':
        return 'XMLTV';
      case 'url':
        return 'HDHR URL';
      case 'streams':
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
        );
      default:
        return header ? header : camel2title(field);
    }
  };

  const getStyle = useCallback((style: CSSProperties | undefined, fieldType: ColumnFieldType | undefined): CSSProperties | undefined => {

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

  const linkIcon = (url: string) => {
    return (
      <a href={url} rel="noopener noreferrer" target="_blank">
        <i className="pi pi-bookmark-fill" />
      </a>
    );
  };


  const linkTemplate = (link: string) => (
    <div>
      <div className="flex justify-content-center align-items-center">
        {linkIcon(link)}
      </div>
    </div>
  );

  const getRecord = useCallback((data: T, fieldName: string) => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];

    return record;
  }, []);

  const getRecordString = useCallback((data: T, fieldName: string): string => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    let toDisplay = JSON.stringify(record);

    if (!toDisplay || toDisplay === undefined || toDisplay === '') {
      // console.log("toDisplay is empty for " + fieldName)
      return '';
    }


    toDisplay = removeQuotes(toDisplay);

    return toDisplay;
  }, []);

  const epgSourceTemplate = useCallback((tvgid: string) => {
    return (
      <div>
        <div className="flex justify-content-start">
          {tvgid}
        </div>
      </div>
    );
  }, []);

  const imageBodyTemplate = (data: T, fieldName: string) => {
    const record = getRecordString(data, fieldName);

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img
          alt={record ?? 'Logo'}
          className="max-h-1rem max-w-full p-0"
          onError={(e: SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  };


  const streamsBodyTemplate = (activeCount: string, totalCount: string) => {
    if (activeCount === null || totalCount === undefined) {
      return null;
    }

    return (
      <div className="flex align-items-center gap-2" >
        {activeCount}/{totalCount}
      </div>
    );

  };


  const bodyTemplate = (data: T, fieldName: string, fieldType: ColumnFieldType, camelize?: boolean) => {

    if (fieldName === undefined || fieldName === '') {
      return <div />;
    }

    // Helper function for 'isHidden' fieldType
    const renderIsHidden = (record: boolean) => {
      if (record !== true) {
        return <i className="pi pi-eye text-green-500" />;
      }

      return <i className="pi pi-eye-slash text-red-500" />;
    };

    // Simplify the rendering logic using a switch statement
    switch (fieldType) {
      case 'blank':
        return <div />;
      case 'm3ulink':
        return linkTemplate(getRecordString(data, 'm3ULink'));
      case 'epglink':
        return linkTemplate(getRecordString(data, 'xmlLink'));
      case 'url':
        return linkTemplate(getRecordString(data, 'hdhrLink'));
      case 'epg':
        return epgSourceTemplate(getRecordString(data, 'user_Tvg_ID'));
      case 'image':
        return imageBodyTemplate(data, fieldName);
      case 'streams':
        const activeCount = getRecord(data, 'activeCount');
        const totalCount = getRecord(data, 'totalCount');

        return streamsBodyTemplate(activeCount, totalCount);
      case 'isHidden':
        return renderIsHidden(getRecord(data, fieldName));
      case 'deleted':
        const toDisplay = getRecord(data, 'isHidden');

        return (
          <span className={`flex ${toDisplay !== true ? 'bg-green-900' : 'bg-red-900'} min-w-full min-h-full justify-content-center align-items-center`}>
            {toDisplay}
          </span>
        );
      default:
        let displayValue = JSON.stringify(getRecord(data, fieldName));

        if (displayValue.startsWith('"') && displayValue.endsWith('"')) {
          displayValue = displayValue.substring(1, displayValue.length - 1);
        }

        if (camelize) {
          displayValue = camel2title(displayValue);
        }

        return (
          <span style={{ display: 'block', overflow: 'hidden', padding: '0rem !important', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
            {displayValue}
          </span>
        );
    }
  };

  console.log("TableColumns", columns)

  return (
    <>
      {columns.map((col) => (
        <Column
          align={getAlign(col.align, col.fieldType)}
          alignHeader={getAlignHeader(col.align, col.fieldType)}
          body={(e) => col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, col.camelize)}
          editor={col.editor}
          field={col.field}
          filter={getFilter(col.filter, col.fieldType)}
          filterElement={col.filterElement}
          filterMenuStyle={{ width: '14rem' }}
          filterPlaceholder={col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)}
          header={getHeader(col.field, col.header, col.fieldType)}
          hidden={col.isHidden === true || (hideControls === true && getHeader(col.field, col.header, col.fieldType) === 'Actions') ? true : undefined}
          key={!col.fieldType ? col.field : col.field + col.fieldType}
          onCellEditComplete={col.handleOnCellEditComplete}
          showAddButton
          showApplyButton
          showClearButton
          showFilterMatchModes
          showFilterMenu={col.filterElement === undefined}
          showFilterMenuOptions
          showFilterOperator
          sortable={groupRowsBy === undefined || groupRowsBy === '' ? col.sortable : false}
          style={getStyle(col.style, col.fieldType)}
        />
      ))}
    </>
  );
};
