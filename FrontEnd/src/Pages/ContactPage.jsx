import React, { useState } from "react";
import Header from "../components/Header";
import Footer from "../components/Footer";

export default function ContactPage() {
    const [formData, setFormData] = useState({
        fullName: "",
        email: "",
        subject: "",
        message: "",
    });

    const [successMessage, setSuccessMessage] = useState("");
    const [errorMessage, setErrorMessage] = useState("");

    const handleChange = (event) => {
        const { name, value } = event.target;

        setFormData((previousData) => ({
            ...previousData,
            [name]: value,
        }));
    };

    const handleSubmit = (event) => {
        event.preventDefault();

        setSuccessMessage("");
        setErrorMessage("");

        if (
            !formData.fullName.trim() ||
            !formData.email.trim() ||
            !formData.subject.trim() ||
            !formData.message.trim()
        ) {
            setErrorMessage("Please fill in all fields.");
            return;
        }

        setSuccessMessage("Your message has been sent successfully.");

        setFormData({
            fullName: "",
            email: "",
            subject: "",
            message: "",
        });
    };

    return (
        <>
            <Header />

            <main className="flex-1 max-w-[1200px] mx-auto px-6 py-12 w-full">
                <div className="text-center mb-12">
                    <h1 className="text-4xl font-bold mb-4">Contact Us</h1>
                    <p className="text-gray-600 text-lg max-w-2xl mx-auto">
                        Have questions about our tours, bookings, or destinations?
                        Send us a message and our team will get back to you.
                    </p>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-[1fr_1.5fr] gap-10">
                    <section className="border border-gray-300 rounded-2xl p-6 h-fit">
                        <h2 className="text-2xl font-bold mb-6">Get in Touch</h2>

                        <div className="space-y-5">
                            <div>
                                <p className="font-semibold">Email</p>
                                <p className="text-gray-600">support@tourplatform.com</p>
                            </div>

                            <div>
                                <p className="font-semibold">Phone</p>
                                <p className="text-gray-600">+995 555 40 56 71</p>
                            </div>

                            <div>
                                <p className="font-semibold">Location</p>
                                <p className="text-gray-600">Tbilisi, Georgia</p>
                            </div>

                            <div>
                                <p className="font-semibold">Working Hours</p>
                                <p className="text-gray-600">
                                    Monday - Friday, 10:00 - 18:00
                                </p>
                            </div>
                        </div>
                    </section>

                    <section className="border border-gray-300 rounded-2xl p-6">
                        <h2 className="text-2xl font-bold mb-6">Send Message</h2>

                        {successMessage && (
                            <p className="text-green-600 font-semibold mb-4">
                                {successMessage}
                            </p>
                        )}

                        {errorMessage && (
                            <p className="text-red-500 font-semibold mb-4">
                                {errorMessage}
                            </p>
                        )}

                        <form onSubmit={handleSubmit} className="space-y-5">
                            <div>
                                <label className="block font-semibold mb-2">
                                    Full Name
                                </label>
                                <input
                                    type="text"
                                    name="fullName"
                                    value={formData.fullName}
                                    onChange={handleChange}
                                    placeholder="Enter your full name"
                                    className="w-full border border-gray-300 rounded-xl px-4 py-3 outline-none focus:border-[#39bf00]"
                                />
                            </div>

                            <div>
                                <label className="block font-semibold mb-2">
                                    Email
                                </label>
                                <input
                                    type="email"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    placeholder="Enter your email"
                                    className="w-full border border-gray-300 rounded-xl px-4 py-3 outline-none focus:border-[#39bf00]"
                                />
                            </div>

                            <div>
                                <label className="block font-semibold mb-2">
                                    Subject
                                </label>
                                <input
                                    type="text"
                                    name="subject"
                                    value={formData.subject}
                                    onChange={handleChange}
                                    placeholder="What is this about?"
                                    className="w-full border border-gray-300 rounded-xl px-4 py-3 outline-none focus:border-[#39bf00]"
                                />
                            </div>

                            <div>
                                <label className="block font-semibold mb-2">
                                    Message
                                </label>
                                <textarea
                                    name="message"
                                    value={formData.message}
                                    onChange={handleChange}
                                    placeholder="Write your message..."
                                    rows="6"
                                    className="w-full border border-gray-300 rounded-xl px-4 py-3 outline-none focus:border-[#39bf00] resize-none"
                                />
                            </div>

                            <button
                                type="submit"
                                className="cursor-pointer w-full rounded-xl bg-[#8dfc9c] px-4 py-3 font-semibold hover:bg-[#39bf00] transition-all duration-300"
                            >
                                Send Message
                            </button>
                        </form>
                    </section>
                </div>
            </main>

            <Footer />
        </>
    );
}