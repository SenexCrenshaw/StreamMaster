import getRecordString from './getRecordString';

export function imageBodyTemplate(data: object, fieldName: string, defaultIcon: string) {
  const record = getRecordString(data, fieldName);

  return (
    <div className="flex icon-template align-content-center justify-content-center align-items-center">
      <img
        alt={record ?? 'Logo'}
        src={`${encodeURI(record ?? '')}`}
        style={{
          objectFit: 'contain'
        }}
        loading="lazy"
      />
    </div>
  );
}
