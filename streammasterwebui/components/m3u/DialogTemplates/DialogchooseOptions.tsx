export const chooseOptions = {
  className: 'p-button-rounded p-button-info',
  icon: 'pi pi-fw pi-plus',
  label: 'Add File'
};

export const uploadOptions = {
  className: 'p-button-rounded p-button-success',
  icon: 'pi pi-fw pi-upload',
  label: 'Upload'
};

export const cancelOptions = {
  className: 'p-button-rounded p-button-danger',
  icon: 'pi pi-fw pi-times',
  label: 'Remove File'
};

export const emptyTemplate = () => (
  <div className="flex align-items-center justify-content-center">
    <i
      className="pi pi-file mt-3 p-5"
      style={{
        backgroundColor: 'var(--surface-b)',
        borderRadius: '50%',
        color: 'var(--surface-d)',
        fontSize: '5em'
      }}
    />
    <span className="my-5" style={{ color: 'var(--text-color-secondary)', fontSize: '1.2em' }}>
      Drag and Drop M3U Here
    </span>
  </div>
);
