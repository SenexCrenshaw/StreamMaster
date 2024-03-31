import getRecordString from './getRecordString';

export function imageBodyTemplate(data: object, fieldName: string, defaultIcon: string) {
  const record = getRecordString(data, fieldName);

  return (
    <div className="iconselector flex align-contents-center w-full min-w-full">
      <img
        alt={record ?? 'Logo'}
        className="max-h-1rem max-w-full p-0"
        src={`${encodeURI(record ?? '')}`}
        style={{
          objectFit: 'contain'
        }}
        loading="lazy"
      />
    </div>
  );
}
