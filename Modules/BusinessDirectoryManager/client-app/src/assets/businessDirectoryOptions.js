export const membershipStatusOptions = [
    { value: 1, text: 'Current' },
    { value: 2, text: 'Pending' },
    { value: 3, text: 'Former' }
];

export function getMembershipStatusText(value) {
    const match = membershipStatusOptions.find(x => x.value === Number(value));
    return match ? match.text : 'Unknown';
}