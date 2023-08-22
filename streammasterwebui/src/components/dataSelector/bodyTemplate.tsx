import { type SyntheticEvent } from "react";
import { camel2title, removeQuotes } from "../../common/common";
import { type ColumnFieldType } from "./DataSelectorTypes";



function linkIcon(url: string) {
  return (
    <a href={url} rel="noopener noreferrer" target="_blank">
      <i className="pi pi-bookmark-fill" />
    </a>
  );
}


function linkTemplate(link: string) {
  <div>
    <div className="flex justify-content-center align-items-center">
      {linkIcon(link)}
    </div>
  </div>
}


function getRecord(data: object, fieldName: string) {
  type ObjectKey = keyof typeof data;
  const record = data[fieldName as ObjectKey];
  return record;
}

function getRecordString(data: object, fieldName: string): string {
  type ObjectKey = keyof typeof data;
  const record = data[fieldName as ObjectKey];
  let toDisplay = JSON.stringify(record);

  if (!toDisplay || toDisplay === undefined || toDisplay === '') {
    // console.log("toDisplay is empty for " + fieldName)
    return '';
  }

  toDisplay = removeQuotes(toDisplay);

  return toDisplay;
}

function epgSourceTemplate(tvgid: string) {
  return (
    <div>
      <div className="flex justify-content-start">
        {tvgid}
      </div>
    </div>
  );
}

function imageBodyTemplate(data: object, fieldName: string, defaultIcon: string) {
  const record = getRecordString(data, fieldName);

  return (
    <div className="flex flex-nowrap justify-content-center align-items-center p-0">
      <img
        alt={record ?? 'Logo'}
        className="max-h-1rem max-w-full p-0"
        onError={(e: SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = defaultIcon))}
        src={`${encodeURI(record ?? '')}`}
        style={{
          objectFit: 'contain',
        }}
      />
    </div>
  );
}

function streamsBodyTemplate(activeCount: string, totalCount: string) {
  if (activeCount === null || totalCount === undefined) {
    return null;
  }

  return (
    <div className="flex align-items-center gap-2" >
      {activeCount}/{totalCount}
    </div>
  );

}

function bodyTemplate(data: object, fieldName: string, fieldType: ColumnFieldType, defaultIcon: string, camelize?: boolean) {

  if (fieldName === undefined || fieldName === '') {
    return <div />;
  }



  // Helper function for 'isHidden' fieldType
  const renderIsHidden = (record: boolean) => {
    if (record !== true) {
      return <i className="pi pi-eye text-green-500" />;
    }

    return <i className="pi pi-eye-slash text-blue-500" />;
  };

  // Simplify the rendering logic using a switch statement
  switch (fieldType) {
    case 'blank':
      return <div />;
    case 'm3ulink':
      { linkTemplate(getRecordString(data, 'm3ULink')); return; }

    case 'epglink':
      { linkTemplate(getRecordString(data, 'xmlLink')); return; }

    case 'url':
      { linkTemplate(getRecordString(data, 'hdhrLink')); return; }

    case 'epg':
      return epgSourceTemplate(getRecordString(data, 'user_Tvg_ID'));
    case 'image':
      return imageBodyTemplate(data, fieldName, defaultIcon);
    case 'streams':
      const activeCount = getRecord(data, 'activeCount');
      const totalCount = getRecord(data, 'totalCount');
      return streamsBodyTemplate(activeCount, totalCount);
    case 'isHidden':
      return renderIsHidden(getRecord(data, fieldName));
    case 'deleted':
      const toDisplay = getRecord(data, 'isHidden');
      return (
        <span className={`flex ${toDisplay !== true ? 'bg-green-900' : 'bg-blue-900'} min-w-full min-h-full justify-content-center align-items-center`}>
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
}

export default bodyTemplate;
